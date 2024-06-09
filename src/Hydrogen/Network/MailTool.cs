// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

/// <summary>
/// Tools for mailing.
/// </summary>
public static class Mail {
	public const int SMTPPort = 25;
	public const int SMTPSubmissionPort = 587;
	public const int SMTPAlternatePort = 2525;
	public const int SecureSMTPPort = 465;
	public const int POP3 = 110;
	public const int SecurePOP3 = 995;
	public const int IMAP4 = 143;
	public const int SecureImap4 = 993;

	private static readonly Regex EmailValidatorRegex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");


	/// <summary>
	/// Validates email address syntax, not whether it actually exists or not.
	/// </summary>
	/// <param name="email">Email address to validate</param>
	/// <returns>Whether email is validly formed</returns>
	public static bool IsValidEmail(string email) {
		return !string.IsNullOrWhiteSpace(email) && EmailValidatorRegex.IsMatch(email);
	}

	public static void SendEmail(
		string smtpServer,
		string from,
		string subject,
		string body,
		string toUser,
		IEnumerable<string> ccRecipients = null,
		IEnumerable<string> bccRecipients = null,
		IEnumerable<string> replyToRecipients = null,
		IEnumerable<Attachment> attachments = null,
		IEnumerable<AlternateView> alternativeViews = null,
		bool requiresSSL = false,
		string username = null,
		string password = null,
		string domain = null,
		int? port = null,
		bool bodyHtml = false
	) {
		SendEmail(
			smtpServer,
			from,
			subject,
			body,
			new[] { toUser },
			ccRecipients,
			bccRecipients,
			replyToRecipients,
			attachments,
			alternativeViews,
			requiresSSL,
			username,
			password,
			domain,
			port
		);
	}


	public static void SendEmail(
		string smtpServer,
		string from,
		string subject,
		string body,
		IEnumerable<string> toRecipients,
		IEnumerable<string> ccRecipients = null,
		IEnumerable<string> bccRecipients = null,
		IEnumerable<string> replyToRecipients = null,
		IEnumerable<Attachment> attachments = null,
		IEnumerable<AlternateView> alternativeViews = null,
		bool requiresSSL = false,
		string username = null,
		string password = null,
		string domain = null,
		int? port = null,
		bool bodyHtml = false
	) {

		#region Pre-Conditions

		Guard.ArgumentNotNull(toRecipients, nameof(toRecipients));
		Guard.ArgumentNotNull(from, nameof(from));

		var badEmails = new List<string>();
		toRecipients
			.Union(ccRecipients ?? System.Array.Empty<string>())
			.Union(bccRecipients ?? System.Array.Empty<string>())
			.Union(replyToRecipients ?? System.Array.Empty<string>())
			.ForEach(u => {
				if (!IsValidEmail(u)) {
					badEmails.Add(u);
				}
			});

		if (badEmails.Count > 0)
			throw new SoftwareException("Unable to send email as TO, CC BCC and/or REPLY-TO included the following bad emails {0}", badEmails.ToDelimittedString(", "));

		#endregion

		port ??= requiresSSL ? SecureSMTPPort : SMTPPort;

		var requiresLogon = !string.IsNullOrEmpty(username);

		var mailMessage = new MailMessage { Subject = subject, Body = body, IsBodyHtml = bodyHtml, From = new MailAddress(@from) };

		toRecipients.Distinct().ForEach(u => mailMessage.To.Add(new MailAddress(u)));

		ccRecipients?.Distinct().ForEach(u => mailMessage.CC.Add(new MailAddress(u)));

		bccRecipients?.Distinct().ForEach(u => mailMessage.Bcc.Add(new MailAddress(u)));

		replyToRecipients?.Distinct().ForEach(u => mailMessage.ReplyToList.Add(new MailAddress(u)));

		attachments?.ForEach(a => mailMessage.Attachments.Add(a));

		alternativeViews?.ForEach(av => mailMessage.AlternateViews.Add(av));

		var smtpClient = new SmtpClient(smtpServer, port.Value);
		if (requiresSSL) {
			smtpClient.EnableSsl = true;
			ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
		}
		if (requiresLogon) {
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Credentials = new NetworkCredential(username, password, domain);
		}

		smtpClient.Send(mailMessage);
	}

	public static Task SendEmailAsync(
		string smtpServer,
		string from,
		string subject,
		string body,
		IEnumerable<string> toRecipients,
		IEnumerable<string> ccRecipients = null,
		IEnumerable<string> bccRecipients = null,
		IEnumerable<string> replyToRecipients = null,
		IEnumerable<Attachment> attachments = null,
		IEnumerable<AlternateView> alternativeViews = null,
		bool requiresSSL = false,
		string username = null,
		string password = null,
		string domain = null,
		int? port = null,
		bool bodyHtml = false
	) {

		#region Pre-Conditions

		Guard.ArgumentNotNull(toRecipients, nameof(toRecipients));
		Guard.ArgumentNotNull(from, nameof(from));

		var badEmails = new List<string>();
		toRecipients
			.Union(ccRecipients ?? System.Array.Empty<string>())
			.Union(bccRecipients ?? System.Array.Empty<string>())
			.Union(replyToRecipients ?? System.Array.Empty<string>())
			.ForEach(u => {
				if (!IsValidEmail(u)) {
					badEmails.Add(u);
				}
			});

		if (badEmails.Count > 0)
			throw new SoftwareException("Unable to send email as TO, CC BCC and/or REPLY-TO included the following bad emails {0}", badEmails.ToDelimittedString(", "));

		#endregion

		port ??= requiresSSL ? SecureSMTPPort : SMTPPort;

		var requiresLogon = !string.IsNullOrEmpty(username);

		var mailMessage = new MailMessage { Subject = subject, Body = body, IsBodyHtml = bodyHtml, From = new MailAddress(@from) };

		toRecipients.Distinct().ForEach(u => mailMessage.To.Add(new MailAddress(u)));

		ccRecipients?.Distinct().ForEach(u => mailMessage.CC.Add(new MailAddress(u)));

		bccRecipients?.Distinct().ForEach(u => mailMessage.Bcc.Add(new MailAddress(u)));

		replyToRecipients?.Distinct().ForEach(u => mailMessage.ReplyToList.Add(new MailAddress(u)));

		attachments?.ForEach(a => mailMessage.Attachments.Add(a));

		alternativeViews?.ForEach(av => mailMessage.AlternateViews.Add(av));

		var smtpClient = new SmtpClient(smtpServer, port.Value);
		if (requiresSSL) {
			smtpClient.EnableSsl = true;
			ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
		}

		if (requiresLogon) {
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Credentials = new NetworkCredential(username, password, domain);
		}

		return smtpClient.SendMailAsync(mailMessage);
	}

	public static IEnumerable<string> ParseEmailsFromCommaSeparatedList(string emails) 
		=> string.IsNullOrWhiteSpace(emails) ? Enumerable.Empty<string>() : emails.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
}

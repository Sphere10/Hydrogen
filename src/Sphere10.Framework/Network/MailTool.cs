//-----------------------------------------------------------------------
// <copyright file="MailTool.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

#if !__WP8__

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Sphere10.Framework;

// ReSharper disable CheckNamespace
namespace Tools {


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
				return EmailValidatorRegex.IsMatch(email);
			}



			public static void SendEmail(
				string smtpServer,
				string from,
				string subject,
				string body,
				string toUser,
				IEnumerable<string> ccUsers = null,
				IEnumerable<string> bccUsers = null,
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
					ccUsers,
					bccUsers,
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
				IEnumerable<string> toUsers,
				IEnumerable<string> ccUsers = null,
				IEnumerable<string> bccUsers = null,
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
				Debug.Assert(toUsers != null);
				Debug.Assert(from != null);
				if (toUsers == null) {
					throw new ArgumentNullException("toUsers");
				}
				if (from == null) {
					throw new ArgumentNullException("from");
				}

				var badEmails = new List<string>();
				toUsers
					.Union(ccUsers ?? new string[0])
					.Union(bccUsers ?? new string[0])
					.ForEach(u => {
						if (!IsValidEmail(u)) {
							badEmails.Add(u);
						}
					});
				if (badEmails.Count > 0) {
					throw new SoftwareException("Unable to send email as TO, CC and/or BCC included the following bad emails {0}", badEmails.ToDelimittedString(", "));
				}

				#endregion

				if (port == null)
					port = requiresSSL ? SecureSMTPPort : SMTPPort;

				bool requiresLogon = !string.IsNullOrEmpty(username);

				var mailMessage = new MailMessage { Subject = subject, Body = body, IsBodyHtml = bodyHtml, From = new MailAddress(@from) };

				toUsers.Distinct().ForEach(u => mailMessage.To.Insert(0, new MailAddress(u)));

				if (ccUsers != null) {
					ccUsers.Distinct().ForEach(u => mailMessage.CC.Insert(0, new MailAddress(u)));
				}

				if (bccUsers != null) {
					bccUsers.Distinct().ForEach(u => mailMessage.Bcc.Insert(0, new MailAddress(u)));
				}

				if (attachments != null) {
					attachments.ForEach(a => mailMessage.Attachments.Add(a));
				}

				if (alternativeViews != null) {
					alternativeViews.ForEach(av => mailMessage.AlternateViews.Add(av));
				}

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

		}

	}


#endif

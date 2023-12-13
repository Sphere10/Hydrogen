using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen.Application;
using Hydrogen.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace HydrogenTester.WinForms.Screens;

public partial class EmailTestScreen : ApplicationScreen {
	private EmailerSettings _settings;

	public EmailTestScreen() {
		InitializeComponent();
	}


	protected override void OnShowFirstTime() {
		base.OnShowFirstTime();
		LoadSettings();
	}


	private async Task SendAsync() {
		SaveSettings();
		await Tools.Mail.SendEmailAsync(
			_settings.SMTPServer,
			_settings.From,
			_settings.Subject,
			_settings.Body,
			Tools.Mail.ParseEmailsFromCommaSeparatedList(_settings.To),
			Tools.Mail.ParseEmailsFromCommaSeparatedList(_settings.CC),
			Tools.Mail.ParseEmailsFromCommaSeparatedList(_settings.BCC),
			Tools.Mail.ParseEmailsFromCommaSeparatedList(_settings.ReplyTo),
			requiresSSL: _settings.SSL,
			username: _settings.SMTPUsername,
			password: _settings.SMTPPassword,
			port: _settings.SMTPPort,
			bodyHtml: _settings.IsHtml
		);
	}

	private void Send() {
		SaveSettings();
		Tools.Mail.SendEmail(
			_settings.SMTPServer,
			_settings.From,
			_settings.Subject,
			_settings.Body,
			Tools.Mail.ParseEmailsFromCommaSeparatedList(_settings.To),
			Tools.Mail.ParseEmailsFromCommaSeparatedList(_settings.CC),
			Tools.Mail.ParseEmailsFromCommaSeparatedList(_settings.BCC),
			Tools.Mail.ParseEmailsFromCommaSeparatedList(_settings.ReplyTo),
			requiresSSL: _settings.SSL,
			username: _settings.SMTPUsername,
			password: _settings.SMTPPassword,
			port: _settings.SMTPPort,
			bodyHtml: _settings.IsHtml
		);
	}

	private void LoadSettings() {
		_settings = UserSettings.Get<EmailerSettings>();
		_fromTextBox.Text = _settings.From;
		_toTextBox.Text = _settings.To;
		_ccTextBox.Text = _settings.CC;
		_bccTextBox.Text = _settings.BCC;
		_replyToTextBox.Text = _settings.ReplyTo;
		_smtpServerTextBox.Text = _settings.SMTPServer;
		_smtpPortIntBox.Value = _settings.SMTPPort;
		_smtpUsernameTextBox.Text = _settings.SMTPUsername;
		_smtpPasswordTextBox.Text = _settings.SMTPPassword;
		_sslCheckBox.Checked = _settings.SSL;
		_isHtmlCheckBox.Checked = _settings.IsHtml;
		_subjectTextBox.Text = _settings.Subject;
		_bodyTextBox.Text = _settings.Body;
	}

	private void SaveSettings() {
		_settings.From = _fromTextBox.Text;
		_settings.To = _toTextBox.Text;
		_settings.CC = _ccTextBox.Text;
		_settings.BCC = _bccTextBox.Text;
		_settings.ReplyTo = _replyToTextBox.Text;
		_settings.SMTPServer = _smtpServerTextBox.Text;
		_settings.SMTPPort = _smtpPortIntBox.Value;
		_settings.SMTPUsername = _smtpUsernameTextBox.Text;
		_settings.SMTPPassword = _smtpPasswordTextBox.Text;
		_settings.SSL = _sslCheckBox.Checked;
		_settings.IsHtml = _isHtmlCheckBox.Checked;
		_settings.Subject = _subjectTextBox.Text;
		_settings.Body = _bodyTextBox.Text;
		_settings.Save();
	}

	public class EmailerSettings : SettingsObject {
		public string From { get; set; }

		public string To { get; set; }

		public string CC { get; set; }

		public string BCC { get; set; }

		public string ReplyTo { get; set; }

		public string SMTPServer { get; set; }

		public int? SMTPPort { get; set; }

		public string SMTPUsername { get; set; }

		public string SMTPPassword { get; set; }

		public bool SSL { get; set; }

		public bool IsHtml { get; set; }

		public string Subject { get; set; }

		public string Body { get; set; }

	}

	private async void _sendButtonAsync_Click(object sender, EventArgs e) {
		try {
			using (_loadingCircle.BeginAnimationScope(this)) {
				await SendAsync();
			}
		} catch (Exception ex) {
			ExceptionDialog.Show(ex);
		}

	}

	private void _sendButton_Click(object sender, EventArgs e) {
		try {
			using (_loadingCircle.BeginAnimationScope(this)) {
				Send();
			}
		} catch (Exception ex) {
			ExceptionDialog.Show(ex);
		}

	}
}


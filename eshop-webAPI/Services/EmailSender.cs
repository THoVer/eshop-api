﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace eshopAPI.Services
{
    public interface IEmailSender
    {
      Task SendEmailAsync(string to, string subject, string message);
      Task SendConfirmationEmailAsync(string to, string confirmationLink);
      Task SendResetPasswordEmailAsync(string to, string resetPasswordLink);
      Task SendOrderCreationEmailAsync(string to, string orderId);
    }

    public class EmailSender : IEmailSender
    {
        private IConfiguration Configuration { get; set; }
        private string Host { get; }
        private int Port { get; }
        private string User { get; }
        private string From { get; }
        private string Password { get; }

        public EmailSender(IConfiguration configuration)
        {
          Configuration = configuration;
          Host = Configuration["EmailSender.Host"];
          Port = Int32.Parse(Configuration["EmailSender.Port"]);
          User = Configuration["EmailSender.User"];
          From = Configuration["EmailSender.From"];
          Password = Configuration["EmailSender.Password"];
        }
        
        public async Task SendEmailAsync(string to, string subject, string message)
        {
            MailMessage mailMessage = new MailMessage();
   
            mailMessage.To.Add(to);
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = true; 

            await SendMailMessageAsync(mailMessage);
        }
 
        public async Task SendConfirmationEmailAsync(string to, string confirmationLink)
        {
          var mailMessage = new MailMessage();
          mailMessage.To.Add(to);
          mailMessage.Subject = "Eshop account activation";
          mailMessage.IsBodyHtml = true;
          mailMessage.Body = FormatEmailBody($"Please click the following link to confirm that <strong> { to }</strong> is your email address which you will use in Goal diggers shop", confirmationLink);
          await SendMailMessageAsync(mailMessage);
        }

      public async Task SendResetPasswordEmailAsync(string to, string resetPasswordLink)
      {
        var mailMessage = new MailMessage();
        mailMessage.To.Add(to);
        mailMessage.Subject = "Goals diggers reset password confirmation";
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = FormatEmailBody($"Please click the following link to confirm that <strong> { to }</strong> wants to reset Goal diggers shop password.", resetPasswordLink);
        await SendMailMessageAsync(mailMessage);
      }

      public async Task SendOrderCreationEmailAsync(string to, string orderNumber)
      {
        var mailMessage = new MailMessage();
        mailMessage.To.Add(to);
        mailMessage.Subject = "Goal diggers shop order";
        mailMessage.IsBodyHtml = true;
        mailMessage.Body = $"Your order with number: {orderNumber} was successfuly created.";
        await SendMailMessageAsync(mailMessage);
      }

      private async Task SendMailMessageAsync(MailMessage message)
        {
            message.From = new MailAddress(From);

            using (var client = new SmtpClient(Host, Port)
            {
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential(User, Password)
            })
            {
                await client.SendMailAsync(message);
            }
        }

        private string FormatEmailBody(string message, string link)
        {
            return @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
  
  <head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Revue</title>
    <style type=""text/css"">
      #outlook a {padding:0;}
      body{width:100% !important; -webkit-text-size-adjust:100%; -ms-text-size-adjust:100%; margin:0; padding:0;-webkit-font-smoothing: antialiased;-moz-osx-font-smoothing: grayscale;} 
      .ExternalClass {width:100%;}
      .ExternalClass, .ExternalClass p, .ExternalClass span, .ExternalClass font, .ExternalClass td, .ExternalClass div, .ExternalClass blockquote {line-height: 100%;}
      .ExternalClass p, .ExternalClass blockquote {margin-bottom: 0; margin: 0;}
      #backgroundTable {margin:0; padding:0; width:100% !important; line-height: 100% !important;}
      
      img {outline:none; text-decoration:none; -ms-interpolation-mode: bicubic;} 
      a img {border:none;} 
      .image_fix {display:block;}
  
      p {margin: 1em 0;}
  
      h1, h2, h3, h4, h5, h6 {color: black !important;}
      h1 a, h2 a, h3 a, h4 a, h5 a, h6 a {color: black;}
      h1 a:active, h2 a:active,  h3 a:active, h4 a:active, h5 a:active, h6 a:active {color: black;}
      h1 a:visited, h2 a:visited,  h3 a:visited, h4 a:visited, h5 a:visited, h6 a:visited {color: black;}
  
      table td {border-collapse: collapse;}
      table { border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt; }
  
      a {color: #3498db;}
      p.domain a{color: black;}
  
      hr {border: 0; background-color: #d8d8d8; margin: 0; margin-bottom: 0; height: 1px;}
  
      @media (max-device-width: 667px) {
        a[href^=""tel""], a[href^=""sms""] {
          text-decoration: none;
          color: blue;
          pointer-events: none;
          cursor: default;
        }
  
        .mobile_link a[href^=""tel""], .mobile_link a[href^=""sms""] {
          text-decoration: default;
          color: orange !important;
          pointer-events: auto;
          cursor: default;
        }
  
        h1[class=""profile-name""], h1[class=""profile-name""] a {
          font-size: 32px !important;
          line-height: 38px !important;
          margin-bottom: 14px !important;
        }
  
        span[class=""issue-date""], span[class=""issue-date""] a {
          font-size: 14px !important;
          line-height: 22px !important;
        }
  
        td[class=""description-before""] {
          padding-bottom: 28px !important;
        }
        td[class=""description""] {
          padding-bottom: 14px !important;
        }
        td[class=""description""] span, span[class=""item-text""], span[class=""item-text""] span {
          font-size: 16px !important;
          line-height: 24px !important;
        }
  
        span[class=""item-link-title""] {
          font-size: 18px !important;
          line-height: 24px !important;
        }
  
        span[class=""item-header""] {
          font-size: 22px !important;
        }
  
        span[class=""item-link-description""], span[class=""item-link-description""] span {
          font-size: 14px !important;
          line-height: 22px !important;
        }
  
        .link-image {
          width: 84px !important;
          height: 84px !important;
        }
  
        .link-image img {
          max-width: 100% !important;
          max-height: 100% !important;
        }
  
      }
  
      @media only screen and (min-device-width: 768px) and (max-device-width: 1024px) {
        a[href^=""tel""], a[href^=""sms""] {
          text-decoration: none;
          color: blue;
          pointer-events: none;
          cursor: default;
        }
  
        .mobile_link a[href^=""tel""], .mobile_link a[href^=""sms""] {
          text-decoration: default;
          color: orange !important;
          pointer-events: auto;
          cursor: default;
        }
      }
    </style>
    <!--[if gte mso 9]>
      <style type=""text/css"">
        #contentTable {
          width: 600px;
        }
      </style>
    <![endif]-->
  </head>
  
  <body style=""width:100% !important;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;"">
    <table cellpadding=""0"" cellspacing=""0"" border=""0"" id=""backgroundTable"" style=""margin:0; padding:0; width:100% !important; line-height: 100% !important; border-collapse:collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;""
    width=""100%"">
      <tr>
        <td width=""10"" valign=""top"">&nbsp;</td>
        <td valign=""top"" align=""center"">
          <!--[if (gte mso 9)|(IE)]>
            <table width=""600"" align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""background-color: #FFF; border-collapse:collapse;mso-table-lspace:0pt;mso-table-rspace:0pt;"">
              <tr>
                <td>
                <![endif]-->
                <table cellpadding=""0"" cellspacing=""0"" border=""0"" align=""center"" style=""width: 100%; max-width: 600px; background-color: #FFF; border-collapse:collapse;mso-table-lspace:0pt;mso-table-rspace:0pt;""
                id=""contentTable"">
                  <tr>
                    <td width=""600"" valign=""top"" align=""center"" style=""border-collapse:collapse;"">
                      <table align='center' border='0' cellpadding='0' cellspacing='0' style='border: 1px solid #E0E4E8;'
                      width='100%'>
                        <tr>
                          <td align='left' style='padding: 56px 56px 28px 56px;' valign='top'>
                            <div style='font-family: ""lato"", ""Helvetica Neue"", Helvetica, Arial, sans-serif; line-height: 28px;font-size: 18px; color: #333;font-weight:bold;'>Hey there!</div>
                          </td>
                        </tr>
                        <tr>
                          <td align='left' style='padding: 0 56px 28px 56px;' valign='top'>
                            <div style='font-family: ""lato"", ""Helvetica Neue"", Helvetica, Arial, sans-serif; line-height: 28px;font-size: 18px; color: #333;'>

                          "+ message + @"
                          </div>
                          </td>
                        </tr>
                        <tr>
                          <td align='left' style='padding: 0 56px;' valign='top'>
                            <div>
                              <!--[if mso]>
                                <v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word""
                                href=""#""
                                style=""height:44px;v-text-anchor:middle;width:250px;"" arcsize=""114%"" stroke=""f""
                                fillcolor=""#E15718"">
                                  <w:anchorlock/>
                                <![endif]-->
                                <a style=""background-color:#E15718;border-radius:50px;color:#ffffff;display:inline-block;font-family: &#39;lato&#39;, &#39;Helvetica Neue&#39;, Helvetica, Arial, sans-serif;font-size:18px;line-height:44px;text-align:center;text-decoration:none;width:250px;-webkit-text-size-adjust:none;""
                                href="
          + link +
          @">Confirm address</a>
                                <!--[if mso]>
                                </v:roundrect>
                              <![endif]-->
                            </div>
                          </td>
                          <tr>
                            <td align='left' style='padding: 28px 56px 28px 56px;' valign='top'></td>
                          </tr>
                        </tr>
                      </table>
                      <table align='center' border='0' cellpadding='0' cellspacing='0' width='100%'>
                        <tr>
                          <td align='center' style='padding: 30px 56px 28px 56px;' valign='middle'>
<span style='font-family: ""lato"", ""Helvetica Neue"", Helvetica, Arial, sans-serif; line-height: 28px;font-size: 16px; color: #A7ADB5; vertical-align: middle;'>If this email doesn't make any sense, please <a href=""mailto:" + From + @""">let us know</a>!</span>

                          </td>
                        </tr>
                        <tr>
                          <td align='center' style='padding: 0 56px 28px 56px;' valign='middle'>
                            <a style=""border: 0;"" href=""https://www.getrevue.co/?utm_campaign=Reset+password&utm_content=logo&utm_medium=email&utm_source=reset_password"">
                            </a>
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
                <!--[if (gte mso 9)|(IE)]>
                </td>
              </tr>
            </table>
          <![endif]-->
        </td>
        <td width=""10"" valign=""top"">&nbsp;</td>
      </tr>
    </table>
    
  </body>

</html>
";
        }
    }
}

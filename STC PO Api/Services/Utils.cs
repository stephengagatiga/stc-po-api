using DinkToPdf;
using DinkToPdf.Contracts;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json;
using STC_PO_Api.Context;
using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public class Utils : IUtils
    {

        private IConfiguration _configuration;
        private IWebHostEnvironment _hostingEnvironment;
        private IConverter _converter;
        private int MAX_LINES_READ_IN_TXT_FILE = 20;
        private CultureInfo EN_US = new CultureInfo("en-US");


        public Utils(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IConverter converter)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _converter = converter;
        }
        public byte[] ConvertToPdf(PDFPO po)
        {

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.Letter,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
            };

            return Convert(po, globalSettings);
        }
        public byte[] ConvertToPdf(PDFPO po, string output)
        {

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.Letter,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = output
            };

            return Convert(po, globalSettings);

        }
        public void SendPOApproval(POPending pOPending)
        {
            try
            {
                //Smtp Server  
                string SmtpServer = "smtp.live.com";
                //Smtp Port Number  
                int SmtpPortNumber = 587;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("STC Pipeline", "powerbi@shellsoft.com.ph"));
                message.To.Add(new MailboxAddress(pOPending.ApproverName, pOPending.ApproverEmail));
                message.Subject = "PO #" + pOPending.Id.ToString() + " requires your approval";

                StringBuilder messageBuilder = new StringBuilder();

                messageBuilder.Append(@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                    </head>
                    <body>
                ");
                messageBuilder.Append(POEmailTemplate(pOPending));
                messageBuilder.AppendFormat(EN_US, @"<p style='width: 100%; height: 22px; text-align: left;'>&nbsp;<span style='font-family: arial, helvetica, sans-serif;'>Requested by: {0} </span></p>", pOPending.CreatedByName);
                messageBuilder.AppendFormat(EN_US, @"
                <table style='border-collapse: collapse; width: 100%;' border='0'>
                <tbody>
                <tr>
                <td style='width: 33.3333%; text-align: center;'><span style='font-family: arial, helvetica, sans-serif;'><a href='{0}/l/reject/{1}' target='_blank' rel='noopener' aria-invalid='true'>Reject</a></span></td>
                <td style='width: 33.3333%;'>&nbsp;</td>
                <td style='width: 33.3333%; text-align: center;'><span style='font-family: arial, helvetica, sans-serif;'><a href='{0}/l/approve/{1}' target='_blank' rel='noopener' aria-invalid='true'>Approve</a></span></td>
                </tr>
                </tbody>
                </table>
                <p><span style='color: #7e8c8d; font-size: 10pt; font-family: arial, helvetica, sans-serif;'><em>This is system generated message, please do not reply.</em></span></p>
                </body>
                </html>", _configuration.GetValue<string>("CloudAPI"), pOPending.Guid);

                var builder = new BodyBuilder();
                builder.HtmlBody = messageBuilder.ToString();

                foreach (var item in pOPending.POAttachments)
                {
                    builder.Attachments.Add(item.Name, item.File);
                }


                // Now we just need to set the message body and we're done
                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {

                    //client.Connect("smtp.live.com", 587, false);
                    ////client.AuthenticationMechanisms.Remove("XOAUTH2");
                    //client.Authenticate("powerbi@shellsoft.com.ph", "*3-hO3Lr");
                    ////client.Capabilities &= ~SmtpCapabilities.Pipelining;
                    ////client.Authenticate("stchelpdesk@shellsoft.com.ph", "+a5razAw");
                    //client.Send(message);
                    //client.Disconnect(true);

                    //client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

                    //client.ServerCertificateValidationCallback = (mysender, certificate, chain, sslPolicyErrors) => { return false; };
                    //client.CheckCertificateRevocation = false;

                    //client.SslProtocols = System.Security.Authentication.SslProtocols.None;

                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    //client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate("powerbi@shellsoft.com.ph", "*3-hO3Lr");
                    //client.Capabilities &= ~SmtpCapabilities.Pipelining;
                    //client.Authenticate("stchelpdesk@shellsoft.com.ph", "+a5razAw");
                    client.Send(message);
                    client.Disconnect(true);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Send Email Message Failed!");
                Console.WriteLine(e);
                throw e;
            }
        }
        public void SendPOCancellation(POPending pOPending, UpdatePOStatus updatePOStatus)
        {
            try
            {

                //Smtp Server  
                string SmtpServer = "smtp.live.com";
                //Smtp Port Number  
                int SmtpPortNumber = 587;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("STC Pipeline", "powerbi@shellsoft.com.ph"));
                message.To.Add(new MailboxAddress(updatePOStatus.ApproverName, updatePOStatus.ApproverEmail));
                message.Subject = "Cancellation of PO #" + pOPending.OrderNumber.ToString() + " requires your approval";

                StringBuilder messageBuilder = new StringBuilder();

                messageBuilder.Append(@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                    </head>
                    <body>
                ");

                messageBuilder.AppendFormat(EN_US, @"
                <table style='border-collapse: collapse; width: 100%; height: 88px;' border='0'>
                <tbody>
                    <tr style='height: 22px;'>
                        <td style='width: 16.7055%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'>{0} wants to cancel this with the following reason:</span></td>
                    </tr>
                    <tr style='height: 22px;'>
                        <td style='width: 16.7055%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'>{1}</span></td>
                    </tr>
                </tbody>
                </table>
                <br/>
                <br/>
                <p>Below are the details of the PO:</p>
                ", updatePOStatus.Requestor, updatePOStatus.Message);

                messageBuilder.Append(POEmailTemplate(pOPending));

                messageBuilder.AppendFormat(EN_US, @"<p style='width: 100%; height: 22px; text-align: left;'>&nbsp;<span style='font-family: arial, helvetica, sans-serif;'>Approver: {0} </span></p>", pOPending.ApproverName);

                messageBuilder.AppendFormat(EN_US, @"
                <p>&nbsp;</p>
                <table style='border-collapse: collapse; width: 100%;' border='0'>
                <tbody>
                <tr>
                <td style='width: 33.3333%; text-align: center;'><span style='font-family: arial, helvetica, sans-serif;'><a href='{0}/l/cancel/reject/{1}' target='_blank' rel='noopener' aria-invalid='true'>Reject</a></span></td>
                <td style='width: 33.3333%;'>&nbsp;</td>
                <td style='width: 33.3333%; text-align: center;'><span style='font-family: arial, helvetica, sans-serif;'><a href='{0}/l/cancel/approve/{1}' target='_blank' rel='noopener' aria-invalid='true'>Approve</a></span></td>
                </tr>
                </tbody>
                </table>
                <p><span style='color: #7e8c8d; font-size: 10pt; font-family: arial, helvetica, sans-serif;'><em>This is system generated message, please do not reply.</em></span></p>
                </body>
                </html>", _configuration.GetValue<string>("CloudAPI"), pOPending.Guid);

                var builder = new BodyBuilder();
                builder.HtmlBody = messageBuilder.ToString();

                // We may also want to attach a calendar event for Monica's party...
                //builder.Attachments.Add(@"C:\Users\phen.STC-SJSG\Documents\Apps.xlsx");

                // Now we just need to set the message body and we're done
                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    client.Authenticate("powerbi@shellsoft.com.ph", "*3-hO3Lr");
                    //client.Authenticate("stchelpdesk@shellsoft.com.ph", "+a5razAw");
                    client.Send(message);
                    client.Disconnect(true);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Send Email Message Failed!");
                Console.WriteLine(e);
                throw e;
            }
        }
        public void SendUpdateToApprover(POPending pOPending, string approverEmail, ICollection<POAttachment> attachments)
        {
            try
            {
                //Smtp Server  
                string SmtpServer = "smtp.live.com";
                //Smtp Port Number  
                int SmtpPortNumber = 587;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("STC Pipeline", "powerbi@shellsoft.com.ph"));

                foreach (var item in GetApproverEmails())
                {
                    message.To.Add(new MailboxAddress("Approver", item));
                }

                message.Subject = "PO #" + pOPending.Id.ToString(EN_US) + " has been updated";

                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.Append(@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                    </head>
                    <body>
                ");
                messageBuilder.AppendFormat(EN_US, @"<p>Below are the details of the updated PO:</p>");
                messageBuilder.Append(POEmailTemplate(pOPending));
                messageBuilder.AppendFormat(EN_US, @"<p style='width: 100%; height: 22px; text-align: left;'>&nbsp;<span style='font-family: arial, helvetica, sans-serif;'>Approver: {0} </span></p>", pOPending.ApproverName);

                var builder = new BodyBuilder();
                builder.HtmlBody = messageBuilder.ToString();

                if (attachments != null)
                {
                    foreach (var item in attachments)
                    {
                        builder.Attachments.Add(item.Name, item.File);
                    }
                }
                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    client.Authenticate("powerbi@shellsoft.com.ph", "*3-hO3Lr");
                    //client.Authenticate("stchelpdesk@shellsoft.com.ph", "+a5razAw");
                    client.Send(message);
                    client.Disconnect(true);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Send Email Message Failed!");
                Console.WriteLine(e);
                throw e;
            }
        }
        void OnMessageSent(object sender, MessageSentEventArgs e)
        {
            Console.WriteLine("The message was sent!");
        }
        public CultureInfo getCultureInfo()
        {
            return EN_US;
        }
        public void SendPOEmailUpdates(POPending po, string subject)
        {
            try
            {
                //Smtp Server  
                string SmtpServer = "smtp.live.com";
                //Smtp Port Number  
                int SmtpPortNumber = 587;

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("STC Pipeline", "powerbi@shellsoft.com.ph"));

                foreach (var item in GetGroupsEmailsThatPOUpdatesShouldNotify())
                {
                    message.To.Add(new MailboxAddress("",item));
                }

                message.Subject = subject;
                StringBuilder messageBuilder = new StringBuilder();
                messageBuilder.Append(@"
                  <html xmlns:v='urn:schemas-microsoft-com:vml' xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns:m='http://schemas.microsoft.com/office/2004/12/omml' xmlns='http://www.w3.org/TR/REC-html40'><head><meta http-equiv=Content-Type content='text/html; charset=us-ascii'><meta name=Generator content='Microsoft Word 15 (filtered medium)'><style><!--
    /* Font Definitions */
    @font-face
        {font-family:Wingdings;
        panose-1:5 0 0 0 0 0 0 0 0 0;}
    @font-face
        {font-family:'Cambria Math';
        panose-1:2 4 5 3 5 4 6 3 2 4;}
    @font-face
        {font-family:Calibri;
        panose-1:2 15 5 2 2 2 4 3 2 4;}
    /* Style Definitions */
    p.MsoNormal, li.MsoNormal, div.MsoNormal
        {margin:0cm;
        margin-bottom:.0001pt;
        font-size:11.0pt;
        font-family:'Calibri',sans-serif;
        mso-fareast-language:EN-US;}
    a:link, span.MsoHyperlink
        {mso-style-priority:99;
        color:#0563C1;
        text-decoration:underline;}
    a:visited, span.MsoHyperlinkFollowed
        {mso-style-priority:99;
        color:#954F72;
        text-decoration:underline;}
    p.MsoListParagraph, li.MsoListParagraph, div.MsoListParagraph
        {mso-style-priority:34;
        margin-top:0cm;
        margin-right:0cm;
        margin-bottom:0cm;
        margin-left:36.0pt;
        margin-bottom:.0001pt;
        font-size:11.0pt;
        font-family:'Calibri',sans-serif;
        mso-fareast-language:EN-US;}
    span.EmailStyle17
        {mso-style-type:personal-compose;
        font-family:'Calibri',sans-serif;
        color:windowtext;}
    .MsoChpDefault
        {mso-style-type:export-only;
        font-family:'Calibri',sans-serif;
        mso-fareast-language:EN-US;}
    @page WordSection1
        {size:612.0pt 792.0pt;
        margin:72.0pt 72.0pt 72.0pt 72.0pt;}
    div.WordSection1
        {page:WordSection1;}
    /* List Definitions */
    @list l0
        {mso-list-id:793132327;
        mso-list-type:hybrid;
        mso-list-template-ids:1979735294 873005057 873005059 873005061 873005057 873005059 873005061 873005057 873005059 873005061;}
    @list l0:level1
        {mso-level-number-format:bullet;
        mso-level-text:\F0B7;
        mso-level-tab-stop:none;
        mso-level-number-position:left;
        text-indent:-18.0pt;
        font-family:Symbol;}
    @list l0:level2
        {mso-level-number-format:bullet;
        mso-level-text:o;
        mso-level-tab-stop:none;
        mso-level-number-position:left;
        text-indent:-18.0pt;
        font-family:'Courier New';}
    @list l0:level3
        {mso-level-number-format:bullet;
        mso-level-text:\F0A7;
        mso-level-tab-stop:none;
        mso-level-number-position:left;
        text-indent:-18.0pt;
        font-family:Wingdings;}
    @list l0:level4
        {mso-level-number-format:bullet;
        mso-level-text:\F0B7;
        mso-level-tab-stop:none;
        mso-level-number-position:left;
        text-indent:-18.0pt;
        font-family:Symbol;}
    @list l0:level5
        {mso-level-number-format:bullet;
        mso-level-text:o;
        mso-level-tab-stop:none;
        mso-level-number-position:left;
        text-indent:-18.0pt;
        font-family:'Courier New';}
    @list l0:level6
        {mso-level-number-format:bullet;
        mso-level-text:\F0A7;
        mso-level-tab-stop:none;
        mso-level-number-position:left;
        text-indent:-18.0pt;
        font-family:Wingdings;}
    @list l0:level7
        {mso-level-number-format:bullet;
        mso-level-text:\F0B7;
        mso-level-tab-stop:none;
        mso-level-number-position:left;
        text-indent:-18.0pt;
        font-family:Symbol;}
    @list l0:level8
        {mso-level-number-format:bullet;
        mso-level-text:o;
        mso-level-tab-stop:none;
        mso-level-number-position:left;
        text-indent:-18.0pt;
        font-family:'Courier New';}
    @list l0:level9
        {mso-level-number-format:bullet;
        mso-level-text:\F0A7;
        mso-level-tab-stop:none;
        mso-level-number-position:left;
        text-indent:-18.0pt;
        font-family:Wingdings;}
    ol
        {margin-bottom:0cm;}
    ul
        {margin-bottom:0cm;}
    --></style><!--[if gte mso 9]><xml>
    <o:shapedefaults v:ext='edit' spidmax='1026' />
    </xml><![endif]--><!--[if gte mso 9]><xml>
    <o:shapelayout v:ext='edit'>
    <o:idmap v:ext='edit' data='1' />
    </o:shapelayout></xml><![endif]--></head>
    <body lang=EN-PH link='#0563C1' vlink='#954F72'>
    <div class=WordSection1>
                ");
                messageBuilder.AppendFormat(EN_US, @"<p class=MsoNormal>Please see the updates regarding this PO No. {0}<o:p></o:p></p>", po.Id);
                messageBuilder.Append("<p>&nbsp;</p>");
                messageBuilder.Append(POEmailTemplate(po));
                
                messageBuilder.Append(@"<p class=MsoNormal><o:p>&nbsp;</o:p></p><p class=MsoNormal><b>Audit trail logs<o:p></o:p></b></p><ul style='margin-top:0cm' type=disc>");
                
                foreach (var item in po.POAuditTrails.OrderByDescending(d => d.DateAdded))
                {
                    messageBuilder.AppendFormat(EN_US, @"<li class=MsoListParagraph style='margin-left:0cm;mso-list:l0 level1 lfo1'>
                        <span style='color:#7F7F7F;mso-style-textfill-fill-color:#7F7F7F;mso-style-textfill-fill-alpha:100.0%'>{0} &#8211; {1}</span>
                        <br>
                        {2}
                        <br>
                        <o:p></o:p>
                    </li>", item.UserName, item.DateAdded.ToString("MM/dd/yyyy", EN_US), item.Message.Replace("\n","<br>"));
                }

                messageBuilder.Append(@"</ul><p class=MsoNormal><o:p>&nbsp;</o:p></p>");
                
   
                
                messageBuilder.Append("<p class=MsoNormal><o:p>&nbsp;</o:p></p><p class=MsoNormal><o:p>&nbsp;</o:p></p><p class=MsoNormal><o:p>&nbsp;</o:p></p><p class=MsoNormal><i><span style='font-size:10.0pt;color:#7F7F7F;mso-style-textfill-fill-color:#7F7F7F;mso-style-textfill-fill-alpha:100.0%'>This is system generated message please do not reply.<o:p></o:p></span></i></p></div></body></html>");

                var builder = new BodyBuilder();
                builder.HtmlBody = messageBuilder.ToString();

                message.Body = builder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect(SmtpServer, SmtpPortNumber, false);
                    client.Authenticate("powerbi@shellsoft.com.ph", "*3-hO3Lr");
                    //client.Authenticate("stchelpdesk@shellsoft.com.ph", "+a5razAw");
                    client.Send(message);
                    client.Disconnect(true);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Send Email Message Failed!");
                Console.WriteLine(e);
                throw e;
            }

        }
        public ICollection<string> GetApproverEmails()
        {
            var approvedLines = new string[MAX_LINES_READ_IN_TXT_FILE];
            var approvedEmails = new List<string>();

            try
            {
                //only allocate memory here
                using (StreamReader sr = System.IO.File.OpenText(_hostingEnvironment.ContentRootPath + "\\group-access\\approved-notif.txt"))
                {
                    int x = 0;
                    while (!sr.EndOfStream)
                    {
                        approvedLines[x] = sr.ReadLine();
                        x += 1;
                    }
                } //CLOSE THE FILE because we are now DONE with it.
                Parallel.For(0, approvedLines.Length, x =>
                {
                    if (!String.IsNullOrEmpty(approvedLines[x]))
                    {
                        approvedEmails.Add(approvedLines[x]);
                    }
                });
            } catch(Exception ex)
            {
                throw ex;
            }

            return approvedEmails;
        }
        public ICollection<string> GetGroupsEmailsWithReadPermission()
        {
            var readLines = new string[MAX_LINES_READ_IN_TXT_FILE];
            var readEmails = new List<string>();

            try
            {
                //only allocate memory here
                using (StreamReader sr = System.IO.File.OpenText(_hostingEnvironment.ContentRootPath + "\\group-access\\read.txt"))
                {
                    int x = 0;
                    while (!sr.EndOfStream)
                    {
                        readLines[x] = sr.ReadLine();
                        x += 1;
                    }
                } //CLOSE THE FILE because we are now DONE with it.
                Parallel.For(0, readLines.Length, x =>
                {
                    if (!String.IsNullOrEmpty(readLines[x]))
                    {
                        readEmails.Add(readLines[x]);
                    }
                });

            } catch(Exception ex)
            {
                throw ex;
            }

            return readEmails;
        }
        public ICollection<string> GetGroupsEmailsWithModifyPermission()
        {
            var modifyLines = new string[MAX_LINES_READ_IN_TXT_FILE];
            var modifyEmails = new List<string>();

            try
            {
                //only allocate memory here
                using (StreamReader sr = System.IO.File.OpenText(_hostingEnvironment.ContentRootPath + "\\group-access\\creator.txt"))
                {
                    int x = 0;
                    while (!sr.EndOfStream)
                    {
                        modifyLines[x] = sr.ReadLine();
                        x += 1;
                    }
                } //CLOSE THE FILE because we are now DONE with it.
                Parallel.For(0, modifyLines.Length, x =>
                {
                    if (!String.IsNullOrEmpty(modifyLines[x]))
                    {
                        modifyEmails.Add(modifyLines[x]);
                    }
                });

            } catch(Exception ex)
            {
                throw ex;
            }

            return modifyEmails;
        }
        public ICollection<string> GetGroupsEmailsWithReceiverPermission()
        {
            var receiverLines = new string[20];
            var receiverEmails = new List<string>();

            try
            {
                //only allocate memory here
                using (StreamReader sr = System.IO.File.OpenText(_hostingEnvironment.ContentRootPath + "\\group-access\\receiver.txt"))
                {
                    int x = 0;
                    while (!sr.EndOfStream)
                    {
                        receiverLines[x] = sr.ReadLine();
                        x += 1;
                    }
                } //CLOSE THE FILE because we are now DONE with it.
                Parallel.For(0, receiverLines.Length, x =>
                {
                    if (!String.IsNullOrEmpty(receiverLines[x]))
                    {
                        receiverEmails.Add(receiverLines[x]);
                    }
                });
            } catch(Exception ex)
            {
                throw ex;
            }

            return receiverEmails;
        }
        public ICollection<string> GetGroupsEmailsThatPOUpdatesShouldNotify()
        {
            var updatesLines = new string[20];
            var updatesEmails = new List<string>();

            try
            {
                //only allocate memory here
                using (StreamReader sr = System.IO.File.OpenText(_hostingEnvironment.ContentRootPath + "\\group-access\\updates-notif.txt"))
                {
                    int x = 0;
                    while (!sr.EndOfStream)
                    {
                        updatesLines[x] = sr.ReadLine();
                        x += 1;
                    }
                } //CLOSE THE FILE because we are now DONE with it.
                Parallel.For(0, updatesLines.Length, x =>
                {
                    if (!String.IsNullOrEmpty(updatesLines[x]))
                    {
                        updatesEmails.Add(updatesLines[x]);
                    }
                });
            } catch(Exception ex)
            {
                throw ex;
            }

            return updatesEmails;
        }
        private byte[] Convert(PDFPO po, GlobalSettings globalSettings)
        {

            var parseItems = JsonConvert.DeserializeObject<NewPOPendingItem[]>(po.POPendingItemsJsonString);
            
            List<POPendingItem> pendingItems = new List<POPendingItem>();
            foreach (var item in parseItems)
            {
                pendingItems.Add(new POPendingItem()
                {
                    Id = 0,
                    POPendingId = 0,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Total = item.Price * item.Quantity,
                    Order = item.Order
                });
            }

            pendingItems.Sort((a, b) => a.Order.CompareTo(b.Order));


            const int LINE_LIMIT = 40; //available lines in the page
            //split the line break count
            //the last index is the line break count of remarks
            string[] linesCountArray = po.TextLineBreakCount.Split(",");
            bool isRemarksRendered = false;
            bool isTotalRendered = false;
            int pageLineLimit = LINE_LIMIT;
            int totalSignatoryLineCount = 7;

            //compute the subtotal
            decimal subtotal = 0;
            foreach (var item in pendingItems)
            {
                subtotal += item.Total;
            }

            int lastItemIndex = 0;


            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings
            };

            bool isThereSomethingToRender = true;
            int pageCounter = 0;
            while (isThereSomethingToRender)
            {
                //this will break the items to distribute in others pages if nescessary
                pageLineLimit = LINE_LIMIT;

                //add pageCounter
                pageCounter++;

                var pdfHtml = new StringBuilder();

                pdfHtml.Append(@"
                    <html>
                    <head>
                    </head>
                    <body>");

                //Header
                pdfHtml.AppendFormat(EN_US, @"
                    <div style='width: 100%; height: 1050px;'>
                        <table class='header'>
                            <colgroup>
                                <col style='width: 400px'>
                                <col>
                                <col style='width: 100%'>
                                <col style='width: 174px'>
                            </colgroup>
                              <tr>
                                <th class='tg-gx32' style='vertical-align: bottom;'><img src='{0}' width='300px' /></th>
                                <th class='tg-7vsa'></th>
                                <th class='tg-gx32'></th>
                                <th class='tg-waqj' rowspan='2' style='vertical-align: middle;'><span style='font-weight:bold'>Purchase Order</span><br></th>
                              </tr>
                              <tr>
                                <th style='vertical-align: top;font-size: 12px; text-align: justify; '><span>7th Floor, Santolan Town Plaza, 276 Santolan Road, San Juan City, 1500, Philippines. Tel: +632 8740 0431</span></th>
                                <th></th>
                                <th></th>
                              </tr>
                        </table>
                        ", _hostingEnvironment.ContentRootPath + "\\css\\shellsoft.png");

                //Supplier & PO # row
                pdfHtml.AppendFormat(EN_US, @"
                            <br/>
                            <table class='PODetails'>
                                <colgroup>
                                    <col style='width: 100px'>
                                    <col style='width: 100%'>
                                    <col style='width: 100px'>
                                    <col style='width: 100px;'>
                                </colgroup>
                                <tr>
                                  <th class='tg-68n1' style='white-space: nowrap;'>Supplier</th>
                                  <th class='tg-3zn0' style='padding-left: 10px'>{0}</th>
                                  <th class='tg-68n1' style='white-space: nowrap;'>PO No.</th>
                                  <th class='tg-3zn0' style='padding-left: 10px'>{1}</th>
                                </tr>
                ", po.SupplierName, po.OrderNumber);

                //Supplier Address & Reference Number row
                pdfHtml.AppendFormat(EN_US, @"
                <tr>
                  <td class='tg-5fi5' style='white-space: nowrap;'>Address</td>
                  <td class='tg-ffik' style='padding-left: 10px' rowspan='2'>{0}</td>
                  <td class='tg-5fi5' style='white-space: nowrap;'>Ref. No.</td>
                  <td class='tg-ffik' style='padding-left: 10px'>{1}</td>
                </tr>", po.SupplierAddress, po.ReferenceNumber);

                //PO Date
                pdfHtml.AppendFormat(EN_US, @"
                <tr>
                  <td class='tg-5fi5' style='white-space: nowrap;'></td>
                  <td class='tg-5fi5' style='white-space: nowrap;'>Date</td>
                  <td class='tg-ffik' style='padding-left: 10px'>{0}</td>
                </tr>", po.ApprovedOn == null ? "" : ((DateTime)po.ApprovedOn).ToString("MM/dd/yyyy", EN_US));

                string terms = "";
                if (po.TermsOfPayment == "COD")
                {
                    terms = String.Format(@"{0}", po.TermsOfPayment);
                }
                else if (po.TermsOfPayment != null && po.TermsOfPayment != "")
                {
                    terms = String.Format(@"{0} days", po.TermsOfPayment);
                }

                //Contact Person & Expected Delivery row
                pdfHtml.AppendFormat(EN_US, @"
                        <tr>
                          <td class='tg-5fi5' style='white-space: nowrap;'>Attention</td>
                          <td class='tg-ffik' style='padding-left: 10px'>{0}</td>
                          <td class='tg-5fi5' style='white-space: nowrap;'>ETA</td>
                          <td class='tg-ffik' style='padding-left: 10px'>{1}</td>
                        </tr>
                        <tr>
                          <td class='tg-5fi5' style='white-space: nowrap;'>S.I# or B.I#</td>
                          <td class='tg-ffik' style='padding-left: 10px'>{2}</td>
                          <td class='tg-5fi5' style='white-space: nowrap;'>Terms</td>
                          <td class='tg-ffik' style='padding-left: 10px'>{3}</td>
                        </tr>
                        <tr>
                          <td class='tg-5fi5' style='white-space: nowrap;'>Invoice Date</td>
                          <td class='tg-ffik' style='padding-left: 10px'>{4}</td>
                          <td class='tg-5fi5' style='white-space: nowrap;'></td>
                          <td class='tg-ffik' style='padding-left: 10px'></td>
                        </tr>
                        </table>", po.ContactPersonName, 
                        po.EstimatedArrival == null ? "" : ((DateTime)po.EstimatedArrival).ToString("MM/dd/yyyy", EN_US),
                        po.SIOrBI, terms, 
                        po.InvoiceDate == null ? "" : ((DateTime)po.InvoiceDate).ToString("MM/dd/yyyy", EN_US));

                //if all order items are not rendered, add heading  
                if (lastItemIndex != pendingItems.Count)
                {
                    pdfHtml.Append(@"
                    <br/>
                        <table class='POItems' style='width: 100%'>
                            <colgroup>
                                 <col style='width: 10%'>
                                <col style='width: 60%'>
                                <col style='width: 15%'>
                                <col style='width: 15%'>
                            </colgroup>
                            <tr>
                                <th class='tg-tn1f' style='padding-top: 10px; padding-bottom: 10px;padding-right: 10px;'>Quantity</th>
                                <th class='tg-tn1f' style='padding-top: 10px; padding-bottom: 10px;'>Description</th>
                                <th class='tg-tn1f' style='padding-top: 10px; padding-bottom: 10px; text-align: center'>Unit Price</th>
                                <th class='tg-tn1f' style='padding-top: 10px; padding-bottom: 10px; text-align: center'>Total</th>
                            </tr>");

                }

                //render the items
                for (int i = lastItemIndex; i < pendingItems.Count; i++, lastItemIndex++)
                {
                    pageLineLimit -= int.Parse(linesCountArray[i], EN_US);

                    //break the loop, so the remaining items will move to another page
                    if (pageLineLimit < 0)
                    {
                        break;
                    }

                    var currentItem = pendingItems.ElementAt(i);
                    decimal amount = currentItem.Quantity * currentItem.Price;
                    pdfHtml.AppendFormat(EN_US, @"
                                <tr>
                                    <td class='tg-z4i2' style='padding-top: 10px; padding-bottom: 10px;text-align: center;'>{0}</td>
                                    <td class='tg-z4i2' style='padding-top: 10px; padding-bottom: 10px;'>{1}</td>
                                    <td class='tg-z4i2' style='padding-left: 20px; text-align: right;padding-top: 10px; padding-bottom: 10px;'>{2}</td>
                                    <td class='tg-z4i2' style='padding-left: 20px; text-align: right; font-weight: bold;padding-top: 10px; padding-bottom: 10px;'>{3}</td>
                                </tr>
                                ", String.Format(EN_US, "{0:n0}", currentItem.Quantity) , currentItem.Name, String.Format(EN_US, "{0:n}", currentItem.Price), String.Format(EN_US, "{0:n}", amount));

                    //get the last index so that it will continue from where it left during break in the loop of items
                }

                //prevent from rendering again the total
                if (!isTotalRendered)
                {
                    //if all order items are rendered, render total 
                    if (lastItemIndex == pendingItems.Count)
                    {
                        //if the total is not fit in the remaining line move it to another page
                        if (pageLineLimit <= 5)
                        {
                            pdfHtml.Append(@"
                             </body>
                            </html>");

                            var objectSettings2 = new ObjectSettings
                            {
                                PagesCount = true,
                                HtmlContent = pdfHtml.ToString(),
                                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = _hostingEnvironment.ContentRootPath + "\\css\\po.css" },
                            };

                            pdf.Objects.Add(objectSettings2);

                            continue;
                        }

                        //add heading if the total is the first item in the page so the total will have style
                        if (pageLineLimit == LINE_LIMIT)
                        {
                            pdfHtml.Append(@"
                            <br/>
                                <table class='POItems' style='width: 100%'>
                                    <colgroup>
                                         <col style='width: 10%'>
                                        <col style='width: 60%'>
                                        <col style='width: 15%'>
                                        <col style='width: 15%'>
                                    </colgroup>
                                    <tr>
                                        <th class='tg-tn1f' style='padding-top: 10px; padding-bottom: 10px;padding-right: 10px;'>Quantity</th>
                                        <th class='tg-tn1f' style='padding-top: 10px; padding-bottom: 10px;'>Description</th>
                                        <th class='tg-tn1f' style='padding-top: 10px; padding-bottom: 10px;text-align: center;'>Unit Price</th>
                                        <th class='tg-tn1f' style='padding-top: 10px; padding-bottom: 10px;text-align: center;'>Total</th>
                                    </tr>");

                        }

                        decimal d = Math.Round((decimal)(po.Discount / 100) * subtotal, 2);
                        //Render the total
                        pdfHtml.AppendFormat(EN_US, @"
                                <tr>
                                    <td class='tg-0w69' colspan='3'></td>
                                    <td class='tg-0w69' style='padding-left: 20px; text-align: right;font-weight: bold;padding-top: 10px; padding-bottom: 10px;'>{0}</td>
                                </tr>
                                <tr>
                                    <td class='tg-0w69' colspan='3' style='text-align: right;padding-top: 10px; padding-bottom: 10px;'>Discount {1}%</td>
                                    <td class='tg-0w69' style='padding-left: 20px; text-align: right;font-weight: bold;padding-top: 10px; padding-bottom: 10px;'>-{2}</td>
                                </tr>
                                <tr>
                                    <td class='tg-0w69' colspan='3' style='text-align: right;font-weight: bold;padding-top: 10px; padding-bottom: 10px;'>Total Amount</td>
                                    <td class='tg-0w69' style='padding-left: 50px; text-align: right;font-weight: bold;padding-top: 10px; padding-bottom: 10px;border-top: 1px solid black;white-space: nowrap;'>{3}{4}</td>
                                </tr>
                            </table>", String.Format(EN_US, "{0:n}", subtotal), po.Discount, String.Format(EN_US, "{0:n}", d),
                          po.Currency, 
                          String.Format(EN_US, "{0:n}", subtotal - d ));

                        //5 is the number lines that are needed to render the total
                        pageLineLimit -= 5;
                        isTotalRendered = true;
                    }
                }

                //prevent from rendering again the remarks
                if (!isRemarksRendered)
                {
                    int remarksLinetCount = int.Parse(linesCountArray[linesCountArray.Length - 1], EN_US);

                    //if all order items are rendered, render the remarks 
                    if (lastItemIndex == pendingItems.Count)
                    {
                        //if the remarks & signatory is not fit in the remaining line move it to another page
                        //+6 is the number lines of signatory
                        if (pageLineLimit < (remarksLinetCount + totalSignatoryLineCount))
                        {
                            pdfHtml.Append(@"
                             </body>
                            </html>");

                            var objectSettings2 = new ObjectSettings
                            {
                                PagesCount = true,
                                HtmlContent = pdfHtml.ToString(),
                                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = _hostingEnvironment.ContentRootPath + "\\css\\po.css" },
                            };

                            pdf.Objects.Add(objectSettings2);

                            continue;
                        }


                        //add space between remarks and items
                        pdfHtml.Append(@"<p>&nbsp;</p>");
                        pdfHtml.Append(@"<p>&nbsp;</p>");


                        pdfHtml.AppendFormat(EN_US, @"
                        <p style='width: 800px;white-space: pre-wrap;'><strong>{0}</strong><br/>{1}</p>
                        ", String.IsNullOrEmpty(po.Remarks) ? "" : "Note:", po.Remarks);

                        pageLineLimit -= remarksLinetCount;
                        isRemarksRendered = true;

                    }
                }

                //check if all are rendered
                if (isRemarksRendered && isTotalRendered)
                {

                    //for (int i = 0; i < pageLineLimit; i++)
                    //{
                    //    pdfHtml.Append(@"<p>&nbsp;</p>");
                    //}

                    //add space between remarks and singatures
                    pdfHtml.Append(@"<p>&nbsp;</p>");
                    pdfHtml.Append(@"<p>&nbsp;</p>");

                    pdfHtml.AppendFormat(EN_US, @"
                        </div>
                            <table class='signature' style='width: 100%; max-height: 100px;'>
                                    <colgroup>
                                        <col style='width: 30%'>
                                        <col style='width: 40%'>
                                        <col style='width: 30%'>
                                    </colgroup>
                                    <tr>
                                      <th class='tg-zv4m'>Approved by:</th>
                                      <th class='tg-zv4m'></th>
                                      <th class='tg-zv4m'>Acknowledge by:</th>
                                    </tr>");

                    if (po.ApprovedOn != null)
                    {

                        string approver = String.Format(EN_US, _configuration.GetValue<string>("HostAPI") + "/po/approver/{0}/signature", po.ApproverId);

                        pdfHtml.AppendFormat(EN_US, @"
                            <tr>
                                <td class='tg-zv4m' style='border-bottom: 1px solid black; vertical-align: bottom;'>
                                    <img src='{0}' style='max-height: 100px; max-width: 300px' />
                                </td>
                                <td class='tg-zv4m'>&nbsp;</td>
                                <td class='tg-zv4m' style='border-bottom: 1px solid black;'>
                                    <div style='width: 300px; height: 100px;'></div>
                                </td>
                            </tr>
                            ", approver);
                    }
                    else
                    {
                        pdfHtml.AppendFormat(EN_US, @"
                            <tr>
                                <td class='tg-zv4m' style='border-bottom: 1px solid black; '>
                                    <div style='width: 300px; height: 100px;'></div>
                                </td>
                                <td class='tg-zv4m'>&nbsp;</td>
                                <td class='tg-zv4m' style='border-bottom: 1px solid black;'>
                                    <div style='width: 300px; height: 100px;'></div>
                                </td>
                            </tr>
                            ");
                    }


                    pdfHtml.AppendFormat(EN_US, @"
                                <tr>
                                  <td class='tg-zv4m'>{0}</td>
                                  <td class='tg-zv4m'></td>
                                  <td class='tg-zv4m'>{1}</td>
                                </tr>
                            <tr>
                                <td class='tg-zv4m'>{2}</td>
                                <td class='tg-zv4m'></td>
                                <td class='tg-zv4m'></td>
                            </tr>
                              </table>
                            ", po.ApproverName, po.ContactPersonName, po.ApproverJobTitle);

                    pdfHtml.Append(@"
                     </body>
                    </html>");

                    //stop the rendering of page
                    isThereSomethingToRender = false;
                }
                else
                {
                    //always close the table
                    pdfHtml.Append(@"</table>");

                    pdfHtml.Append(@"
                     </body>
                    </html>");
                }


                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = pdfHtml.ToString(),
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = _hostingEnvironment.ContentRootPath + "\\css\\po.css" },
                };

                pdf.Objects.Add(objectSettings);
            }

            int tmpCurrentPage = 0;
            foreach (var item in pdf.Objects)
            {
                tmpCurrentPage++;
                item.FooterSettings = new FooterSettings() { FontName = "Arial", FontSize = 9, Right = "Page " + tmpCurrentPage.ToString(EN_US) + " of " + pageCounter.ToString(EN_US) };
            }

            var fileBytes = _converter.Convert(pdf);

            return fileBytes;
        }
        private string POEmailTemplate(POPending pOPending)
        {
            StringBuilder messageBuilder = new StringBuilder();

            messageBuilder.AppendFormat(EN_US, @"
                <table style='border-collapse: collapse; width: 100%; height: 88px;' border='0'>
                <tbody>
                <tr style='height: 22px;'>
                <td style='width: 16.7055%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Supplier</strong></span></td>
                <td style='width: 44.5095%; height: 22px;'>{0}</td>
                <td style='width: 20.4439%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>PO No.</strong></span></td>
                <td style='width: 18.3411%; height: 22px;'>{1}</td>
                </tr>
                ", pOPending.SupplierName, pOPending.OrderNumber);

            messageBuilder.AppendFormat(EN_US, @"
                <tr style='height: 22px;'>
                    <td style='width: 16.7055%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Address</strong></span></td>
                    <td style='width: 44.5095%; height: 22px; vertical-align: top;' rowspan='2'>{0}</td>
                    <td style='width: 20.4439%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Ref No.</strong></span></td>
                    <td style='width: 18.3411%; height: 22px;'>{1}</td>
                </tr>", pOPending.SupplierAddress, pOPending.ReferenceNumber);

            messageBuilder.AppendFormat(EN_US, @"
                <tr style='height: 22px;'>
                <td style='width: 16.7055%; height: 22px;'></td>
                <td style='width: 20.4439%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Date</strong></span></td>
                <td style='width: 18.3411%; height: 22px;'>{0}</td>
                </tr>", pOPending.ApprovedOn == null ? "" : ((DateTime)pOPending.ApprovedOn).ToString("MM/dd/yyyy", EN_US));

            messageBuilder.AppendFormat(EN_US, @"
                <tr style='height: 22px;'>
                <td style='width: 16.7055%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Contact Person</strong></span></td>
                <td style='width: 44.5095%; height: 22px;'>{0}</td>
                <td style='width: 20.4439%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>ETA</strong></span></td>
                <td style='width: 18.3411%; height: 22px;'>{1}</td>
                </tr>", pOPending.ContactPersonName, pOPending.EstimatedArrival == null ? "" : ((DateTime)pOPending.EstimatedArrival).ToString("MM/dd/yyyy", EN_US));

            messageBuilder.AppendFormat(EN_US, @"
                <tr style='height: 22px;'>
                <td style='width: 16.7055%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Customer</strong></span></td>
                <td style='width: 44.5095%; height: 22px;'>{0}</td>
                <td style='width: 20.4439%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Currency</strong></span></td>
                <td style='width: 18.3411%; height: 22px;'>{1}</td>
                </tr>", pOPending.CustomerName, pOPending.Currency);

            string terms = "";
            if (pOPending.TermsOfPayment == "COD")
            {
                terms = String.Format(@"{0}", pOPending.TermsOfPayment);
            }
            else if (pOPending.TermsOfPayment != null && pOPending.TermsOfPayment != "")
            {
                terms = String.Format(@"{0} days", pOPending.TermsOfPayment);
            }

            messageBuilder.AppendFormat(EN_US, @"
                <tr style='height: 22px;'>
                <td style='width: 16.7055%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>S.I# or B.I#</strong></span></td>
                <td style='width: 44.5095%; height: 22px;'>{0}</td>
                <td style='width: 20.4439%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Terms</strong></span></td>
                <td style='width: 18.3411%; height: 22px;'>{1}</td>
                </tr>", pOPending.SIOrBI, terms);

            messageBuilder.AppendFormat(EN_US, @"
                <tr style='height: 22px;'>
                <td style='width: 16.7055%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Invoice Date</strong></span></td>
                <td style='width: 44.5095%; height: 22px;'>{0}</td>
                <td style='width: 20.4439%; height: 22px;'><</td>
                <td style='width: 18.3411%; height: 22px;'></td>
                </tr>", pOPending.InvoiceDate == null ? "" : ((DateTime)pOPending.InvoiceDate).ToString("MM/dd/yyyy", EN_US));

            messageBuilder.AppendFormat(EN_US, @"</tbody></table>");

            messageBuilder.Append(@"<br/><table style='border-collapse: collapse; width: 100%; height: 110px;' border='0'>
                <tbody>
                <tr style='height: 22px;'>
                <td style='width: 8.55188%; text-align: center; height: 22px;background-color: #ced4d9;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Quantity</strong></span></td>
                <td style='width: 56.3469%; height: 22px;background-color: #ced4d9;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Description</strong></span></td>
                <td style='width: 17.0143%; height: 22px; text-align: right;background-color: #ced4d9;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Unit Price</strong></span></td>
                <td style='width: 18.087%; height: 22px; text-align: right;background-color: #ced4d9;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Amount</strong></span></td>
                </tr>");

            decimal total = 0;
            foreach (var item in pOPending.POPendingItems.OrderBy(i => i.Order))
            {
                decimal amount = item.Quantity * item.Price;
                messageBuilder.AppendFormat(EN_US, @"
                    <tr style='height: 22px;'>
                    <td style='width: 8.55188%; text-align: center; height: 22px;border-bottom: 1px solid black;'>{0}</td>
                    <td style='width: 56.3469%; height: 22px;border-bottom: 1px solid black;'>{1}</td>
                    <td style='width: 17.0143%; height: 22px; text-align: right;border-bottom: 1px solid black;'>{2}</td>
                    <td style='width: 18.087%; height: 22px; text-align: right;border-bottom: 1px solid black;'><strong>{3}</strong></td>
                    </tr>
                    ", String.Format(EN_US, "{0:n0}", item.Quantity), item.Name, String.Format(EN_US, "{0:n}", item.Price), String.Format(EN_US, "{0:n}", amount));
                total += amount;
            }

            messageBuilder.AppendFormat(EN_US, @"<tr style='height: 22px;'>
                <td style='width: 8.55188%; text-align: center; height: 22px;'>&nbsp;</td>
                <td style='width: 56.3469%; height: 22px;'>&nbsp;</td>
                <td style='width: 17.0143%; height: 22px; text-align: right;'>&nbsp;</td>
                <td style='width: 18.087%; height: 22px; text-align: right;'><strong>{0}</strong></td>
                </tr>", String.Format(EN_US, "{0:n}", total));

            messageBuilder.AppendFormat(EN_US, @"
                <tr style='height: 22px;'>
                <td style='width: 8.55188%; text-align: center; height: 22px;'>&nbsp;</td>
                <td style='width: 56.3469%; height: 22px; text-align: right;'></td>
                <td style='width: 17.0143%; height: 22px; text-align: right;'><span style='font-family: arial, helvetica, sans-serif;'>Discount {0}%</span></td>
                <td style='width: 18.087%; height: 22px; text-align: right;'><strong>-{1}</strong></td>
                </tr>", pOPending.Discount, String.Format(EN_US, "{0:n}", (pOPending.Discount / 100) * total));

            messageBuilder.AppendFormat(EN_US, @"
                <tr style='height: 22px;'>
                <td style='width: 8.55188%; text-align: center; height: 22px;'>&nbsp;</td>
                <td style='width: 56.3469%; height: 22px;'>&nbsp;</td>
                <td style='width: 17.0143%; height: 22px; text-align: right;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Total Amount</strong></span></td>
                <td style='width: 18.087%; height: 22px; text-align: right;border-top: 1px solid black;'><strong>{0}{1}</strong></td>
                </tr>
                </tbody>
                </table>", pOPending.Currency, String.Format(EN_US, "{0:n}", total - ((pOPending.Discount / 100) * total)));

            messageBuilder.AppendFormat(EN_US, @"<table style='border-collapse: collapse; width: 100%; height: 132px;' border='0'>
                <tbody>
                <tr style='height: 22px;'>
                <td style='width: 100%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Remarks</strong></span></td>
                </tr>
                <tr style='height: 22px;'>
                <td style='width: 100%; height: 22px;'>{0}</td>
                </tr>
                <tr style='height: 22px;'>
                <td style='width: 100%; height: 22px;'>&nbsp;</td>
                </tr>
                <tr style='height: 22px;'>
                <td style='width: 100%; height: 22px;'><span style='font-family: arial, helvetica, sans-serif;'><strong>Internal note</strong></span></td>
                </tr>
                <tr style='height: 22px;'>
                <td style='width: 100%; height: 22px;'>{1}</td>
                </tr>
                <tr style='height: 22px;'>
                <td></td>
                </tr>
                </tbody>
                </table>", pOPending.Remarks == null ? "" : Regex.Replace(pOPending.Remarks, @"\r\n?|\n", "<br/>"), pOPending.InternalNote == null ? "" : Regex.Replace(pOPending.InternalNote, @"\r\n?|\n", "<br/>"));

            return messageBuilder.ToString();
        }

        public async Task SendPOToDistirubtionListAsync(string[] To, ICollection<POAttachment> attachments, POPending po)
        {

            Task.Run(() =>
            {

                try
                {
                    //Smtp Server  
                    string SmtpServer = "smtp.office365.com";
                    //Smtp Port Number  
                    int SmtpPortNumber = 587;

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("STC Pipeline", "powerbi@shellsoft.com.ph"));

                    var TOs = To;

                    foreach (var t in TOs)
                    {
                        message.To.Add(new MailboxAddress("",t));
                    }

                    message.Subject = "";

                    switch (po.Status)
                    {
                        case POStatus.Approved:
                            message.Subject = "PO #" + po.OrderNumber.ToString() + " has been approved";
                            break;
                        case POStatus.Cancelled:
                            message.Subject = "PO #" + po.OrderNumber.ToString() + " has been cancelled";
                            break;
                    }

                    StringBuilder messageBuilder = new StringBuilder();

                    messageBuilder.Append(@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                    </head>
                    <body>
                ");

                    

                    messageBuilder.Append(POEmailTemplate(po));




                    switch (po.Status)
                    {
                        case POStatus.Approved:
                            messageBuilder.AppendFormat(EN_US, @"<p style='width: 100%; height: 22px; text-align: left;'>&nbsp;<span style='font-family: arial, helvetica, sans-serif;'>Requested by: {0} </span></p>", po.CreatedByName);
                            messageBuilder.AppendFormat(EN_US, @"<p style='width: 100%; height: 22px; text-align: left;'>&nbsp;<span style='font-family: arial, helvetica, sans-serif;'>Approved by: {0} </span></p>", po.ApproverName);
                            break;
                        case POStatus.Cancelled:
                            messageBuilder.AppendFormat(EN_US, @"<p style='width: 100%; height: 22px; text-align: left;'>&nbsp;<span style='font-family: arial, helvetica, sans-serif;'>Cancelled by: {0} </span></p>", po.CancelledByName);
                            messageBuilder.AppendFormat(EN_US, @"<p style='width: 100%; height: 22px; text-align: left;'>&nbsp;<span style='font-family: arial, helvetica, sans-serif;'>Request by: {0} </span></p>", po.LastApprovalRequestByName);
                            messageBuilder.AppendFormat(EN_US, @"<p style='width: 100%; height: 22px; text-align: left;'>&nbsp;<span style='font-family: arial, helvetica, sans-serif;'>Reason: {0} </span></p>", po.Reason);
                            break;
                    }



                    messageBuilder.AppendFormat(@"
                <p>&nbsp;</p>
                <p><span style='color: #7e8c8d; font-size: 10pt; font-family: arial, helvetica, sans-serif;'><em>This is system generated message, please do not reply.</em></span></p>
                </body>
                </html>");

                    var builder = new BodyBuilder();
                    builder.HtmlBody = messageBuilder.ToString();

                    if (attachments != null)
                    {
                        foreach (var item in attachments)
                        {
                            builder.Attachments.Add(item.Name, item.File);
                        }
                    }


                    message.Body = builder.ToMessageBody();

                    using (var client = new SmtpClient())
                    {
                        client.Connect(SmtpServer, SmtpPortNumber, false);
                        client.Authenticate("powerbi@shellsoft.com.ph", "*3-hO3Lr");
                        //client.Authenticate("stchelpdesk@shellsoft.com.ph", "+a5razAw");
                        client.Send(message);
                        client.Disconnect(true);

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Send Email Message Failed!");
                    Console.WriteLine(e);
                    throw e;
                }

            });


            
        }
    }
}

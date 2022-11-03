using STC_PO_Api.Entities.POEntity;
using STC_PO_Api.Models.PO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace STC_PO_Api.Services
{
    public interface IUtils
    {
        void SendPOApproval(POPending pOPending);
        void SendUpdateToApprover(POPending pOPending, string approverEmail, ICollection<POAttachment> attachments);
        void SendPOCancellation(POPending pOPending, UpdatePOStatus updatePOStatus);
        void SendPOEmailUpdates(POPending po, string subject);
        byte[] ConvertToPdf(PDFPO po);
        byte[] ConvertToPdf(PDFPO po, string output);
        ICollection<string> GetApproverEmails();
        ICollection<string> GetGroupsEmailsWithReadPermission();
        ICollection<string> GetGroupsEmailsWithModifyPermission();
        ICollection<string> GetGroupsEmailsWithReceiverPermission();
        ICollection<string> GetGroupsEmailsThatPOUpdatesShouldNotify();
        Task SendPOToDistirubtionListAsync(string[] To, ICollection<POAttachment> attachments, POPending po);
        CultureInfo getCultureInfo();
    }
}

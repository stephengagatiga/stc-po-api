using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using STC_PO_Api.Context;
using STC_PO_Api.Entities.POApproverEntity;
using STC_PO_Api.Models.PO;

namespace STC_PO_Api.Services
{
    public class SqlPOApproverData : IPOApproverData
    {
        private POContext _context;

        public SqlPOApproverData(POContext context)
        {
            _context = context;
        }


        public POApprover AddPOApprover(NewPOApprover newPOApprover)
        {
            if (newPOApprover == null)
            {
                return null;
            }

            POApprover pOApprover = new POApprover
            {
                FirstName = newPOApprover.FirstName,
                LastName = newPOApprover.LastName,
                Email = newPOApprover.Email,
                JobTitle = newPOApprover.JobTitle,
                AmountPHP = newPOApprover.AmountPHP,
                AmountLogicalOperatorPHP = newPOApprover.AmountLogicalOperatorPHP,
                AmountUSD = newPOApprover.AmountUSD,
                AmountLogicalOperatorUSD = newPOApprover.AmountLogicalOperatorUSD,
                AmountEUR = newPOApprover.AmountEUR,
                AmountLogicalOperatorEUR = newPOApprover.AmountLogicalOperatorEUR,
            };

            using (var ms = new MemoryStream())
            {
                newPOApprover.ImageFile.CopyTo(ms);
                var fileBytes = ms.ToArray();
                pOApprover.SignatureImg = fileBytes;
            }

          

            _context.POApprovers.Add(pOApprover);
            _context.Entry(pOApprover).State = Microsoft.EntityFrameworkCore.EntityState.Added;

            _context.SaveChanges();

            return pOApprover;

        }

        public void DeleteApprover(POApprover approver)
        {
            _context.POApprovers.Remove(approver);
            _context.Entry(approver).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;

            _context.SaveChanges();
        }

        public ICollection<POApprover> GetApprovers()
        {
            return _context.POApprovers.OrderBy(p => p.FirstName).Select(p => 
                new POApprover { 
                        Id = p.Id, 
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        JobTitle = p.JobTitle,
                        AmountPHP = p.AmountPHP,
                        AmountLogicalOperatorPHP = p.AmountLogicalOperatorPHP,
                        AmountUSD = p.AmountUSD,
                        AmountLogicalOperatorUSD = p.AmountLogicalOperatorUSD,
                        AmountEUR = p.AmountEUR,
                        AmountLogicalOperatorEUR = p.AmountLogicalOperatorEUR,
                    Email = p.Email
                    }).ToList();
        }

        public POApprover GetPOApprover(int id)
        {
            return _context.POApprovers.FirstOrDefault(p => p.Id == id);
        }

        public POApprover UpdatePOApprover(EditPOApprover editPOApprover, POApprover oldApprover)
        {
            if (editPOApprover == null || oldApprover == null)
            {
                return null;
            }

            oldApprover.FirstName = editPOApprover.FirstName;
            oldApprover.LastName = editPOApprover.LastName;
            oldApprover.Email = editPOApprover.Email;
            oldApprover.JobTitle = editPOApprover.JobTitle;
            oldApprover.AmountPHP = editPOApprover.AmountPHP;
            oldApprover.AmountLogicalOperatorPHP = editPOApprover.AmountLogicalOperatorPHP;
            oldApprover.AmountUSD = editPOApprover.AmountUSD;
            oldApprover.AmountLogicalOperatorUSD = editPOApprover.AmountLogicalOperatorUSD;
            oldApprover.AmountEUR = editPOApprover.AmountEUR;
            oldApprover.AmountLogicalOperatorEUR = editPOApprover.AmountLogicalOperatorEUR;

            if (editPOApprover.ReplaceImg)
            {
                using (var ms = new MemoryStream())
                {
                    editPOApprover.ImageFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    oldApprover.SignatureImg = fileBytes;
                }
            }

            _context.POApprovers.Update(oldApprover);
            _context.Entry(oldApprover).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            _context.SaveChanges();

            return oldApprover;
        }
    }
}

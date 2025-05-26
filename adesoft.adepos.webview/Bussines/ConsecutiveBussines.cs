using adesoft.adepos.webview.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adesoft.adepos.webview.Bussines
{
    public class ConsecutiveBussines
    {
        public AdeposDBContext _dbcontext { get; set; }
        public ConsecutiveBussines(AdeposDBContext context)
        { 
            this._dbcontext = context;
        }

        public TransactionGeneric GetConsecutive(TransactionGeneric transactionGeneric)
        {
            #region ObtenerConsecutivo
            transactionGeneric.TransactionIsOk = true;
            TypeTransaction Typetrans = _dbcontext.TypeTransactions.Where(x => x.TypeTransactionId == transactionGeneric.TypeTransactionId).First();
            long consecutivenow = 0;
            if (_dbcontext.TransactionGenerics.Where(x => x.TypeTransactionId == transactionGeneric.TypeTransactionId).Count() > 0)
                consecutivenow = _dbcontext.TransactionGenerics.Where(x => x.TypeTransactionId == transactionGeneric.TypeTransactionId).Max(x => x.Consecutive);
            if (consecutivenow == 0)
            {
                if (Typetrans.ConecutiveInit == 0)
                {
                    transactionGeneric.TransactionIsOk = false; transactionGeneric.MessageResponse = "El documento no tiene consecutivo automatico configurado.";
                    transactionGeneric.MessageType = "ERROR";
                    return transactionGeneric;
                }
                transactionGeneric.Consecutive = Typetrans.ConecutiveInit;
                transactionGeneric.ConsecutiveChar = Typetrans.Codigo + transactionGeneric.Consecutive;
            }
            else
            {
                transactionGeneric.Consecutive = consecutivenow;
                if (transactionGeneric.Consecutive++ > Typetrans.ConecutiveEnd)
                {
                    if (Typetrans.ConecutiveInit <= consecutivenow)
                    {
                        transactionGeneric.TransactionIsOk = false; transactionGeneric.MessageResponse = "Finalizo el consecutivo.";
                        transactionGeneric.MessageType = "ERROR";
                        return transactionGeneric;
                    }
                }
                transactionGeneric.ConsecutiveChar = Typetrans.Codigo + transactionGeneric.Consecutive;
            }
            #endregion
            return transactionGeneric;
        }


      
    }
}

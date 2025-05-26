using adesoft.adepos.webview.Data.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace adesoft.adepos.webview.Bussines
{
    public class NavigationManagerViewControl
    {
        List<object> listviews = new List<object>();
        public NavigationManagerViewControl()
        {

        }

        public void RegisterViews(object v)
        {
            listviews.Add(v);
        }

        public void UnRegisterView(object v)
        {
            listviews.Remove(v);
        }

       


        public void NotifyToAllViews(object viewnotify , DTOViewDashBoardDistpatch modelChangued)
        {
            foreach (object view in listviews.Where(x => x != viewnotify).ToList())
            {
                try
                {
                    object[] param = new object[] { modelChangued };
                    view.GetType().GetMethod("NotifyChangedModel").Invoke(view, param);
                }
                catch
                {

                }
            }
        }

    }
}

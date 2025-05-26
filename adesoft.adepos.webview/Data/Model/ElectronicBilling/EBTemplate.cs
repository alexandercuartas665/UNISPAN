using DocumentFormat.OpenXml.Office.CustomDocumentInformationPanel;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;

namespace adesoft.adepos.webview.Data.Model.ElectronicBilling
{
    public static class EBTemplate
    {
        public static PropertyInfo GetPropertyInfo(string templateName , string columnName)
        {
            PropertyInfo propertyInfo = null;
            string propertyName = ""; 

            switch(templateName)
            {
                case "EBProvision":
                    {
                        switch (columnName.Trim())
                        {
                            case "Date":
                                propertyName = "Date";
                                break;

                            case "Invoice Number":
                                propertyName = "InvoiceNum";
                                break;

                            case "Customer Number":
                                propertyName = "CustomerNum";
                                break;

                            case "Customer":
                                propertyName = "CustomerName";
                                break;

                            case "Obra Number":
                                propertyName = "WorkNo";
                                break;

                            case "Obra":
                                propertyName = "WorkName";
                                break;

                            case "PO":
                                propertyName = "PO";
                                break;

                            case "Administrador de Proyecto":
                                propertyName = "AdminName";
                                break;

                            case "Rent":
                                propertyName = "Rent";
                                break;

                            case "Cargos Adicionales":
                                propertyName = "AdditionalCharges";
                                break;

                            case "Producto Charges":
                                propertyName = "ProductCharges";
                                break;

                            case "Acta":
                                propertyName = "RequiredActa";
                                break;

                            default:
                                break;  
                        }

                        if (!string.IsNullOrEmpty(propertyName))
                            propertyInfo = typeof(SalesInvoice).GetProperty(propertyName);

                        break;
                    }

                default:
                    break;
            }

            return propertyInfo;
        }
    }
}

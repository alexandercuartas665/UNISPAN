using System.Collections.Generic;
using System.Data;

namespace adesoft.adepos.webview.Data.Interfaces
{
    public interface ISqlManagerService
    {
        //public void DropTable(string tableName);

        //public EntityModel CreateTable(EntityModel entityModel, bool isTemporary);

        //public void AlterColumnSize(EntityColumnModel entityColumn);

        //public void AddColumn(EntityColumnModel entityColumnModel);

        public int ExecuteNoQuery(string queryString);

        //public List<Dictionary<string, object>> ExecuteQuery(EntityModel entity, List<FileTemplateMappingModel> fileTemplateMappings, string importFileId, RequiredLevel requiredLevel, bool includeSystemFields, bool includeValidationResults);

        //public List<Dictionary<string, object>> ExecuteQuery(EntityModel entity, List<FileTemplateMappingModel> fileTemplateMappings, string importFileId, FilterLevel filterLevel, bool includeSystemFields, bool includeValidationResults);

        public DataTable ExecuteQuery(string queryString, string connectionStringName);

        //public int DeleteRecords(EntityModel entityModel, string importFileId);

        public int DeleteRecords(string tableName);

        public bool BulkData(DataTable dataTable, string tableName, string connectionStringName);

        //public bool UpdateBulkData(EntityModel entityModel, DataTable dataTable);

        //public int CountErrors(EntityModel entity, string importFileId, RequiredLevel requiredLevel = RequiredLevel.None);

        //public int CountRecords(EntityModel entity, string importFileId);
    }
}

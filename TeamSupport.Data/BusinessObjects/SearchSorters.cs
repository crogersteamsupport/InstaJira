using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class SearchSorter
    {
    }

    public partial class SearchSorters
    {
        public void LoadByUserID(int userID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM dbo.SearchSorters WHERE UserID = @UserID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@UserID", userID);
                Fill(command, "SearchSorters");
            }
        }

        public string ConvertToOrderByClause(ref string tempItemsTableFieldsDefinition,
                                             ref string tempItemsTableFields,
                                             ref string selectTicketsFields,
                                             ref string selectWikisFields,
                                             ref string selectNotesFields,
                                             ref string selectProductVersionsFields,
                                             ref string selectTasksFields,
                                             ref string selectWaterCoolerFields)
        {
            StringBuilder resultBuilder = new StringBuilder();

            if (this.Count > 0)
            {
                StringBuilder tempItemsTableFieldsDefinitionBuilder = new StringBuilder();
                StringBuilder tempItemsTableFieldsBuilder = new StringBuilder();
                StringBuilder selectTicketsFieldsBuilder = new StringBuilder();
                StringBuilder selectWikisFieldsBuilder = new StringBuilder();
                StringBuilder selectNotesFieldsBuilder = new StringBuilder();
                StringBuilder selectProductVersionsFieldsBuilder = new StringBuilder();
                StringBuilder selectTasksFieldsBuilder = new StringBuilder();
                StringBuilder selectWaterCoolerFieldsBuilder = new StringBuilder();

                WikiArticlesView wikiArticleViewFields = new WikiArticlesView(base.LoginUser);
                wikiArticleViewFields.LoadColumnNames();

                resultBuilder.Append(" ORDER BY ");

                ReportTableFields ticketsViewFields = new ReportTableFields(base.LoginUser);
                ticketsViewFields.LoadByReportTableID(10);

                for (int i = 0; i < this.Count; i++)
                {
                    if (i > 0)
                    {
                        resultBuilder.Append(", ");
                    }

                    string fieldName = ticketsViewFields.FindByReportTableFieldID(this[i].FieldID).FieldName;
                    resultBuilder.Append("[" + fieldName + "] " + (this[i].Descending ? "DESC" : string.Empty));

                    if (!string.Equals(fieldName, "DateModified", StringComparison.OrdinalIgnoreCase))
                    {
                        string fieldType = ticketsViewFields.FindByReportTableFieldID(this[i].FieldID).DataType;
                        if (string.Equals(fieldType, "varchar", StringComparison.OrdinalIgnoreCase))
                        {
                            fieldType += "(8000)";
                        }
                        tempItemsTableFieldsDefinitionBuilder.Append(", " + fieldName + " " + fieldType);
                        tempItemsTableFieldsBuilder.Append(", " + fieldName);
                        selectTicketsFieldsBuilder.Append(", tv." + fieldName);

                        string wikiEquivalentFieldName = DataUtils.GetWikiEquivalentFieldName(fieldName);

                        if (!DataUtils.GetIsColumnInBaseCollection(wikiArticleViewFields, wikiEquivalentFieldName))
                        {
                            selectWikisFieldsBuilder.Append(", NULL AS " + fieldName);
                        }
                        else
                        {
                            selectWikisFieldsBuilder.Append(", wav." + wikiEquivalentFieldName + " AS " + fieldName);
                        }

                        string notesEquivalentFieldName = DataUtils.GetNotesEquivalentFieldName(fieldName);

                        NotesView notesView = new NotesView(base.LoginUser);
                        NotesViewItem notesViewItem = notesView.AddNewNotesViewItem();
                        if (!DataUtils.GetIsColumnInBaseCollection(notesViewItem.Collection, notesEquivalentFieldName))
                        {
                            selectNotesFieldsBuilder.Append(", NULL AS " + fieldName);
                        }
                        else
                        {
                            selectNotesFieldsBuilder.Append(", nv." + notesEquivalentFieldName + " AS " + fieldName);
                        }

                        string productVersionsEquivalentFieldName = DataUtils.GetProductVersionsEquivalentFieldName(fieldName);

                        ProductVersionsView productVersionsView = new ProductVersionsView(base.LoginUser);
                        ProductVersionsViewItem productVersionsViewItem = productVersionsView.AddNewProductVersionsViewItem();
                        if (!DataUtils.GetIsColumnInBaseCollection(productVersionsViewItem.Collection, productVersionsEquivalentFieldName))
                        {
                            selectProductVersionsFieldsBuilder.Append(", NULL AS " + fieldName);
                        }
                        else
                        {
                            selectProductVersionsFieldsBuilder.Append(", pvv." + productVersionsEquivalentFieldName + " AS " + fieldName);
                        }

                        string tasksEquivalentFieldName = DataUtils.GetTasksEquivalentFieldName(fieldName);

                        TasksView tasksView = new TasksView(base.LoginUser);
                        TasksViewItem tasksViewItem = tasksView.AddNewTasksViewItem();
                        if (!DataUtils.GetIsColumnInBaseCollection(tasksViewItem.Collection, tasksEquivalentFieldName))
                        {
                            selectTasksFieldsBuilder.Append(", NULL AS " + fieldName);
                        }
                        else
                        {
                            selectTasksFieldsBuilder.Append(", tsk." + tasksEquivalentFieldName + " AS " + fieldName);
                        }

                        string waterCoolerEquivalentFieldName = DataUtils.GetWaterCoolerEquivalentFieldName(fieldName);

                        WaterCoolerView waterCoolerView = new WaterCoolerView(base.LoginUser);
                        WaterCoolerViewItem waterCoolerViewItem = waterCoolerView.AddNewWaterCoolerViewItem();
                        if (!DataUtils.GetIsColumnInBaseCollection(waterCoolerViewItem.Collection, waterCoolerEquivalentFieldName))
                        {
                            selectWaterCoolerFieldsBuilder.Append(", NULL AS " + fieldName);
                        }
                        else
                        {
                            selectWaterCoolerFieldsBuilder.Append(", wcv." + waterCoolerEquivalentFieldName + " AS " + fieldName);
                        }
                    }
                }

                tempItemsTableFieldsDefinition = tempItemsTableFieldsDefinitionBuilder.ToString();
                tempItemsTableFields = tempItemsTableFieldsBuilder.ToString();
                selectTicketsFields = selectTicketsFieldsBuilder.ToString();
                selectWikisFields = selectWikisFieldsBuilder.ToString();
                selectNotesFields = selectNotesFieldsBuilder.ToString();
                selectProductVersionsFields = selectProductVersionsFieldsBuilder.ToString();
                selectTasksFields = selectTasksFieldsBuilder.ToString();
                selectWaterCoolerFields = selectWaterCoolerFieldsBuilder.ToString();
            }
            else
            {
                resultBuilder.Append(@" 
          ORDER BY 
            relevance DESC
            , DateModified DESC 
        ");
            }

            return resultBuilder.ToString();
        }
    }

}

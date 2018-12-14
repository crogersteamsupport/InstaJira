using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace TeamSupport.Data
{
    public partial class ForumCategory
    {
    }

    public partial class ForumCategories
    {

        public void LoadCategories(int organizationID, int? productFamilyID = null)
        {
            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder str = new StringBuilder();
                str.Append("SELECT * FROM ForumCategories WHERE OrganizationID = @OrganizationID AND ParentID < 0");

                if (productFamilyID != null)
                {
                    str.Append(" AND (ProductFamilyID IS NULL OR ProductFamilyID = @ProductFamilyID)");
                    command.Parameters.AddWithValue("@ProductFamilyID", productFamilyID);
                }
                str.Append(" ORDER BY Position");

                command.CommandText = str.ToString();
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public void LoadSubcategories(int categoryID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM ForumCategories WHERE ParentID = @ParentID ORDER BY Position";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ParentID", categoryID);
                Fill(command);
            }
        }

        public static string GetCategoryDisplayString(LoginUser loginUser, int categoryID)
        {
            ForumCategories forumCategories = new ForumCategories(loginUser);
            ForumCategories forumParent = new ForumCategories(loginUser);

            forumCategories.LoadByCategoryID(categoryID);
            if (forumCategories.IsEmpty)
                return null;
            else
            {
                forumParent.LoadByCategoryID(forumCategories[0].ParentID);
                if (!forumParent.IsEmpty)
                    return forumParent[0].CategoryName + " -> " + forumCategories[0].CategoryName;
                else
                    return forumCategories[0].CategoryName;
            }

        }

        public void LoadCategoriesAndSubcategories(int organizationID)
        {
            using (SqlCommand command = new SqlCommand())
            {
                command.CommandText = "SELECT * FROM ForumCategories WHERE OrganizationID = @OrganizationID";
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@OrganizationID", organizationID);
                Fill(command);
            }
        }

        public ForumCategory FindByName(string name)
        {
            foreach (ForumCategory forumCategory in this)
            {
                if (forumCategory.CategoryName.ToLower().Trim() == name.ToLower().Trim())
                {
                    return forumCategory;
                }
            }
            return null;
        }
    }

}

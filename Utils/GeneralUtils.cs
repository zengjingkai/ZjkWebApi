using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZjkWebAPIDemo.Utils
{
    /// <summary>
    /// 常用工具方法
    /// </summary>
    public static class GeneralUtils
    {
        /// <summary>
        /// 将Datatable转化为Dictionary集合
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IDictionary<string, string> DatatableToDictionary(DataTable dt)
        {
            IDictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                dic =dt.Rows.Cast<DataRow>().ToDictionary(x => x[0].ToString(), x => x[1].ToString());
            }
            catch (Exception)
            {
               
            }
            return dic;
        }
        /// <summary>
        /// 行转化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T DataRowToObject<T>(DataRow dr) where T : class
        {
            T obj = Activator.CreateInstance<T>();
            string columnName = "";
            foreach (DataColumn dc in dr.Table.Columns)
            {
                columnName = dc.ColumnName;
                try
                {
                    System.Reflection.PropertyInfo pinfo = obj.GetType().GetProperty(columnName.ToUpper());
                    if (pinfo != null)
                    {
                        switch (pinfo.PropertyType.Name.ToLower())
                        {
                            case "string":
                                pinfo.SetValue(obj, dr[columnName].ToString(), null);
                                break;
                            case "double":
                                if (dr[columnName].ToString().Trim() == "")
                                    pinfo.SetValue(obj, 0, null);
                                else
                                    pinfo.SetValue(obj, double.Parse(dr[columnName].ToString()), null);
                                break;
                            case "decimal":
                                if (dr[columnName].ToString() == "")
                                    pinfo.SetValue(obj, 0m, null);
                                else
                                    pinfo.SetValue(obj, decimal.Parse(dr[columnName].ToString()), null);
                                break;
                            case "nullable`1":
                                if (dr[columnName].ToString() == "")
                                    pinfo.SetValue(obj, null, null);
                                else
                                    pinfo.SetValue(obj, dr[columnName], null);
                                break;
                            case "int32":
                                if (dr[columnName].ToString() == "")
                                    pinfo.SetValue(obj, 0, null);
                                else
                                    pinfo.SetValue(obj, int.Parse(dr[columnName].ToString()), null);
                                break;
                            default:
                                pinfo.SetValue(obj, dr[columnName], null);
                                break;
                        }
                    }
                }
                catch
                { }
                columnName = null;
            }
            return obj;
        }
        /// <summary>
        /// DataTable转换为实体类型集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> DataTableToList<T>(DataTable dt) where T : class
        {
            List<T> list = new List<T>();
            if (dt == null || dt.Rows.Count == 0)
                return list;
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(DataRowToObject<T>(dr));
            }
            return list;
        }
        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defvalue"></param>
        /// <param name="decimas"></param>
        /// <returns></returns>
        public static double StringToDouble(string value, double defvalue, int decimas)
        {
            if (string.IsNullOrEmpty(value))
                return defvalue;
            double tmp;
            bool flag = double.TryParse(value, out tmp);
            if (flag)
            {
                if (decimas > -1)
                {
                    return (double)Math.Round((Decimal)tmp, decimas);
                }
                else
                {
                    return tmp;
                }
            }
            else
            {
                return defvalue;
            }
        }
        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double StringToDouble(string value)
        {
            return StringToDouble(value, 0, -1);
        }
        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimas"></param>
        /// <returns></returns>
        public static double StringToDouble(string value, int decimas)
        {
            return StringToDouble(value, 0, decimas);
        }
        /// <summary>
        /// 字符串转double
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defvalue"></param>
        /// <returns></returns>
        public static double StringToDouble(string value, double defvalue)
        {
            return StringToDouble(value, defvalue, -1);
        }
        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? StringToDateTime(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            DateTime tmp;
            bool flag = DateTime.TryParse(value, out tmp);
            if (flag)
                return tmp;
            else
                return null;
        }

        /// <summary>
        /// datatable转化为JSON字符串
        /// </summary>     
        /// <param name="dt">datatable数据集</param>
        /// <returns></returns>
        public static string DataTableToJson( DataTable dt)
        {

            StringBuilder Json = new StringBuilder();
            Json.Append("[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Json.Append(dt.Columns[j].ColumnName.ToString() + ":\"" + dt.Rows[i][j].ToString() + "\"");
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]");
            return Json.ToString();
        }
        /// <summary>
        /// DataTable转化为List
        /// </summary>
        /// <typeparam name="T">对象类</typeparam>
        /// <param name="table">数据集</param>
        /// <returns></returns>
        public static IList<T> ConvertToDataTable<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertTo<T>(rows);
        }

        /// <summary>
        /// 行转List
        /// </summary>
        /// <typeparam name="T">对象类</typeparam>
        /// <param name="rows"></param>
        /// <returns></returns>
        private static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        /// <summary>
        /// 创建类
        /// </summary>
        /// <typeparam name="T">对象类</typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        private static T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        object value = row[column.ColumnName];
                        prop.SetValue(obj, value, null);
                    }
                    catch
                    {  //You can log something here     
                       //throw;    
                    }
                }
            }

            return obj;
        }

        /// <summary>
        /// list集合转JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static string ListToJson<T>(IList<T> ts)
        {
            string json = string.Empty;
            try
            {
                //转化成Json
                json=JsonConvert.SerializeObject(ts);
            }
            catch (Exception)
            {
            }
            return json;
        }

        /// <summary>
        /// JSON转为List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static IList<T> JsonToList<T>(string json)
        {
            IList<T> list = null;
            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    list=new List<T>();
                    //转化成Json
                    list = JsonConvert.DeserializeObject<List<T>>(json);
                }
              
            }
            catch (Exception)
            {
            }
            return list;
        }
    }
}

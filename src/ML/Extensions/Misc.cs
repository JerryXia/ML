using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.CSharp;

namespace ML.Extensions
{
    /// <summary>
    /// var buttons = GetListOfButtons() as IEnumerable&lt;Button&gt;;
    /// <para></para>
    /// <para>// click all buttons</para>
    /// <para>buttons.ForEach(b =&gt; b.Click());</para>
    /// <para></para>
    /// <para></para>
    /// <para>// no need to type the same assignment 3 times, just</para>
    /// <para>// new[] up an array and use foreach + lambda</para>
    /// <para>// everything is properly inferred by csc :-)</para>
    /// <para>new { itemA, itemB, itemC }</para>
    /// <para>    .ForEach(item =&gt; {</para>
    /// <para>        item.Number = 1;</para>
    /// <para>        item.Str = &quot;Hello World!&quot;;</para>
    /// <para>    });</para>
    /// </summary>
    public static partial class ExtensionMethod
    {
        #region Methods

        /// <summary>
        /// 转换匿名类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        /// <param name="example"></param>
        /// <returns></returns>
        public static T TolerantCast<T>(this object o, T example)
            where T : class
        {
            IComparer<string> comparer = StringComparer.CurrentCultureIgnoreCase;
            //Get constructor with lowest number of parameters and its parameters
            var constructor = typeof (T).GetConstructors(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                ).OrderBy(c => c.GetParameters().Length).First();
            var parameters = constructor.GetParameters();

            //Get properties of input object
            var sourceProperties = new List<PropertyInfo>(o.GetType().GetProperties());

            if (parameters.Length > 0)
            {
                var values = new object[parameters.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    Type t = parameters[i].ParameterType;
                    //See if the current parameter is found as a property in the input object
                    var source = sourceProperties.Find(delegate(PropertyInfo item)
                        {
                            return comparer.Compare(item.Name, parameters[i].Name) == 0;
                        });

                    //See if the property is found, is readable, and is not indexed
                    if (source != null && source.CanRead &&
                        source.GetIndexParameters().Length == 0)
                    {
                        //See if the types match.
                        if (source.PropertyType == t)
                        {
                            //Get the value from the property in the input object and save it for use
                            //in the constructor call.
                            values[i] = source.GetValue(o, null);
                            continue;
                        }
                        else
                        {
                            //See if the property value from the input object can be converted to
                            //the parameter type
                            try
                            {
                                values[i] = Convert.ChangeType(source.GetValue(o, null), t);
                                continue;
                            }
                            catch
                            {
                                //Impossible. Forget it then.
                            }
                        }
                    }
                    //If something went wrong (i.e. property not found, or property isn't
                    //converted/copied), get a default value.
                    values[i] = t.IsValueType ? Activator.CreateInstance(t) : null;
                }
                //Call the constructor with the collected values and return it.
                return (T) constructor.Invoke(values);
            }
            //Call the constructor without parameters and return the it.
            return (T) constructor.Invoke(null);
        }


        /// <summary>
        /// 判断元素是否存在于集合中
        /// </summary>
        /// <remarks>
        /// reallyLongIntegerVariableName.In(1,6,9,11)) 
        /// <para>reallyLongStringVariableName.In(&quot;string1&quot;,&quot;string2&quot;,&quot;string3&quot;))</para>
        /// <para>reallyLongMethodParameterName.In(SomeEnum.Value1, SomeEnum.Value2,
        /// SomeEnum.Value3, SomeEnum.Value4</para>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="list"></param>
        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source)
                throw new ArgumentNullException("source elements is null");
            return list.Contains(source);
        }



        /// <summary>
        /// var buttons = GetListOfButtons() as IEnumerable&lt;Button&gt;;
        /// <para></para>
        /// <para>// click all buttons</para>
        /// <para>buttons.ForEach(b =&gt; b.Click());</para>
        /// <para></para>
        /// <para></para>
        /// <para>// no need to type the same assignment 3 times, just</para>
        /// <para>// new[] up an array and use foreach + lambda</para>
        /// <para>// everything is properly inferred by csc :-)</para>
        /// <para>new { itemA, itemB, itemC }</para>
        /// <para>    .ForEach(item =&gt; {</para>
        /// <para>        item.Number = 1;</para>
        /// <para>        item.Str = &quot;Hello World!&quot;;</para>
        /// <para>    });</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="@enum"></param>
        /// <param name="mapFunction"></param>
        public static void ForEach<T>(this IEnumerable<T> @enum, Action<T> mapFunction)
        {
            foreach (var item in @enum) mapFunction(item);
        }


        #region Convert string

        /// <summary>
        /// int i = myString.To&lt;int&gt;();
        /// <para></para>
        /// <para>int i = myString.To&lt;int&gt;();</para>
        /// <para>string a = myInt.ToOrDefault&lt;string&gt;();</para>
        /// <para>//note type inference</para>
        /// <para>DateTime d = myString.ToOrOther(DateTime.MAX_VALUE);</para>
        /// <para>double d;</para>
        /// <para>//note type inference</para>
        /// <para>bool didItGiveDefault = myString.ToOrDefault(out d);</para>
        /// <para>string s = myDateTime.ToOrNull&lt;string&gt;();</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        public static T To<T>(this IConvertible obj)
        {
            return (T) Convert.ChangeType(obj, typeof (T));
        }

        public static T ToOrDefault<T>(this IConvertible obj)
        {
            try
            {
                return To<T>(obj);
            }
            catch
            {
                return default(T);
            }
        }

        public static bool ToOrDefault<T>(this IConvertible obj, out T newObj)
        {
            try
            {
                newObj = To<T>(obj);
                return true;
            }
            catch
            {
                newObj = default(T);
                return false;
            }
        }

        public static T ToOrOther<T>(this IConvertible obj, T other)
        {
            try
            {
                return To<T>(obj);
            }
            catch
            {
                return other;
            }
        }

        public static bool ToOrOther<T>(this IConvertible obj, out T newObj, T other)
        {
            try
            {
                newObj = To<T>(obj);
                return true;
            }
            catch
            {
                newObj = other;
                return false;
            }
        }

        public static T ToOrNull<T>(this IConvertible obj) where T : class
        {
            try
            {
                return To<T>(obj);
            }
            catch
            {
                return null;
            }
        }

        public static bool ToOrNull<T>(this IConvertible obj, out T newObj) where T : class
        {
            try
            {
                newObj = To<T>(obj);
                return true;
            }
            catch
            {
                newObj = null;
                return false;
            }
        }

        #endregion

        /// <summary>
        /// try
        /// <para>{</para>
        /// <para>    //Your stuff here</para>
        /// <para>}</para>
        /// <para>catch(Exception ex)</para>
        /// <para>{</para>
        /// <para>    ex.Log();</para>
        /// <para>}</para>
        /// </summary>
        /// <param name="obj"></param>
        public static void Log(this Exception obj)
        {
            //your logging logic here
        }

        /// <summary>
        /// public Test(string input1) 
        /// <para> {</para>
        /// <para> input1.ThrowIfArgumentIsNull(&quot;input1&quot;);</para>
        /// <para> }</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="parameterName"></param>
        //public static void ThrowIfArgumentIsNull<T>(this T obj, string parameterName) where T : class
        //{
        //    if (obj == null) throw new ArgumentNullException(parameterName + " not allowed to be null");
        //}

        /// <summary>
        /// someVeryVeryLonggggVariableName.With(x =&gt; {
        /// <para>    x.Int = 123;</para>
        /// <para>    x.Str = &quot;Hello&quot;;</para>
        /// <para>    x.Str2 = &quot; World!&quot;;</para>
        /// <para>});</para>
        /// <para></para>
        /// <para>Saves a lot of typing!</para>
        /// <para></para>
        /// <para>Compare this to:</para>
        /// <para></para>
        /// <para>someVeryVeryLonggggVariableName.Int = 123;</para>
        /// <para>someVeryVeryLonggggVariableName.Str = &quot;Hello&quot;;</para>
        /// <para>someVeryVeryLonggggVariableName.Str2 = &quot; World!&quot;;</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="act"></param>
        public static void With<T>(this T obj, Action<T> act)
        {
            act(obj);
        }


        /// <summary>
        /// SomeEntityObject entity = DataAccessObject.GetSomeEntityObject( id );
        /// <para>List&lt;PropertyInfo&gt; properties =
        /// entity.GetType().GetPublicNonCollectionProperties( );</para>
        /// <para></para>
        /// <para>// wordify the property names to act as column headers for an html table
        /// or something</para>
        /// <para>List&lt;string&gt; columns = properties.Select( p =&gt; p.Name.Capitalize(
        /// ).Wordify( ) ).ToList( );</para>
        /// </summary>
        /// <param name="camelCaseWord"></param>
        public static string Wordify(this string camelCaseWord)
        {
            // if the word is all upper, just return it
            if (!Regex.IsMatch(camelCaseWord, "[a-z]"))
                return camelCaseWord;

            return string.Join(" ", Regex.Split(camelCaseWord, @"(?<!^)(?=[A-Z])"));
        }

        public static string Capitalize(this string word)
        {
            return word[0].ToString().ToUpper() + word.Substring(1);
        }

        /// <summary>
        /// double test = 154.20;
        /// <para>string testString = test.ToCurrency(&quot;en-US&quot;); // $154.20</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureName"></param>
        public static string ToCurrency(this double value, string cultureName)
        {
            CultureInfo currentCulture = new CultureInfo(cultureName);
            return (string.Format(currentCulture, "{0:C}", value));
        }


        #region Predicate Builder

        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        public static Expression<Func<T, bool>> False<T>()
        {
            return f => false;
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                      Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                (Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                       Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                (Expression.And(expr1.Body, invokedExpr), expr1.Parameters);
        }

        #endregion


        public static DataTable ToDataTable<T>(this IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names  
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow  
                if (oProps == null)
                {
                    oProps = ((Type) rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof (Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                                                                                      (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }


        public static void ToCSV(this DataTable table, string delimiter, bool includeHeader)
        {
            StringBuilder result = new StringBuilder();
            if (includeHeader)
            {
                foreach (DataColumn column in table.Columns)
                {
                    result.Append(column.ColumnName);
                    result.Append(delimiter);
                }
                result.Remove(--result.Length, 0);
                result.Append(Environment.NewLine);
            }

            foreach (DataRow row in table.Rows)
            {
                foreach (object item in row.ItemArray)
                {
                    if (item is System.DBNull)
                        result.Append(delimiter);
                    else
                    {

                        string itemAsString = item.ToString();
                        // Double up all embedded double quotes 
                        itemAsString = itemAsString.Replace("\"", "\"\"");
                        // To keep things simple, always delimit with double-quotes 
                        // so we don't have to determine in which cases they're necessary 
                        // and which cases they're not. 
                        itemAsString = "\"" + itemAsString + "\"";
                        result.Append(itemAsString + delimiter);
                    }
                }
                result.Remove(--result.Length, 0);
                result.Append(Environment.NewLine);
            }
            using (StreamWriter writer = new StreamWriter(@"C:\log.csv", true))
            {
                writer.Write(result.ToString());

            }
        }

        public static XDocument ToXml(this DataTable dt, string rootName)
        {
            var xdoc = new XDocument
                {
                    Declaration = new XDeclaration("1.0", "utf-8", "")
                };
            xdoc.Add(new XElement(rootName));
            foreach (DataRow row in dt.Rows)
            {
                var element = new XElement(dt.TableName);
                foreach (DataColumn col in dt.Columns)
                {
                    element.Add(new XElement(col.ColumnName, row[col].ToString().Trim(' ')));
                }
                if (xdoc.Root != null) xdoc.Root.Add(element);
            }

            return xdoc;
        }

        public static string ToJson<T>(this T item, System.Text.Encoding encoding = null, System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = null)
        {
            encoding = encoding ?? Encoding.Default;
            serializer = serializer ?? new DataContractJsonSerializer(typeof (T));

            using (var stream = new System.IO.MemoryStream())
            {
                serializer.WriteObject(stream, item);
                var json = encoding.GetString((stream.ToArray()));

                return json;
            }
        }

        public static T Deserialize<T>(this XDocument xmlDocument)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof (T));
            using (XmlReader reader = xmlDocument.CreateReader())
                return (T) xmlSerializer.Deserialize(reader);
        }


        /// <summary> 
        /// Extension method to write list data to excel. 
        /// </summary> 
        /// <typeparam name="T">Ganeric list</typeparam> 
        /// <param name="list"></param> 
        /// <param name="PathToSave">Path to save file.</param> 
        //public static void ToExcel<T>(this List<T> list, string PathToSave)
        //{
        //    #region Declarations

        //    if (string.IsNullOrEmpty(PathToSave))
        //    {
        //        throw new Exception("Invalid file path.");
        //    }
        //    else if (PathToSave.ToLower().Contains("") == false)
        //    {
        //        throw new Exception("Invalid file path.");
        //    }

        //    if (list == null)
        //    {
        //        throw new Exception("No data to export.");
        //    }

        //    Excel.Application excelApp = null;
        //    Excel.Workbooks books = null;
        //    Excel._Workbook book = null;
        //    Excel.Sheets sheets = null;
        //    Excel._Worksheet sheet = null;
        //    Excel.Range range = null;
        //    Excel.Font font = null;
        //    // Optional argument variable 
        //    object optionalValue = Missing.Value;

        //    string strHeaderStart = "A2";
        //    string strDataStart = "A3";
        //    #endregion

        //    #region Processing


        //    try
        //    {
        //        #region Init Excel app.


        //        excelApp = new Excel.Application();
        //        books = (Excel.Workbooks)excelApp.Workbooks;
        //        book = (Excel._Workbook)(books.Add(optionalValue));
        //        sheets = (Excel.Sheets)book.Worksheets;
        //        sheet = (Excel._Worksheet)(sheets.get_Item(1));

        //        #endregion

        //        #region Creating Header


        //        Dictionary<string, string> objHeaders = new Dictionary<string, string>();

        //        PropertyInfo[] headerInfo = typeof(T).GetProperties();


        //        foreach (var property in headerInfo)
        //        {
        //            var attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), false)
        //                                    .Cast<DisplayNameAttribute>().FirstOrDefault();
        //            objHeaders.Add(property.Name, attribute == null ?
        //                                property.Name : attribute.DisplayName);
        //        }


        //        range = sheet.get_Range(strHeaderStart, optionalValue);
        //        range = range.get_Resize(1, objHeaders.Count);

        //        range.set_Value(optionalValue, objHeaders.Values.ToArray());
        //        range.BorderAround(Type.Missing, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);

        //        font = range.Font;
        //        font.Bold = true;
        //        range.Interior.Color = Color.LightGray.ToArgb();

        //        #endregion

        //        #region Writing data to cell


        //        int count = list.Count;
        //        object[,] objData = new object[count, objHeaders.Count];

        //        for (int j = 0; j < count; j++)
        //        {
        //            var item = list[j];
        //            int i = 0;
        //            foreach (KeyValuePair<string, string> entry in objHeaders)
        //            {
        //                var y = typeof(T).InvokeMember(entry.Key.ToString(), BindingFlags.GetProperty, null, item, null);
        //                objData[j, i++] = (y == null) ? "" : y.ToString();
        //            }
        //        }


        //        range = sheet.get_Range(strDataStart, optionalValue);
        //        range = range.get_Resize(count, objHeaders.Count);

        //        range.set_Value(optionalValue, objData);
        //        range.BorderAround(Type.Missing, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);

        //        range = sheet.get_Range(strHeaderStart, optionalValue);
        //        range = range.get_Resize(count + 1, objHeaders.Count);
        //        range.Columns.AutoFit();

        //        #endregion

        //        #region Saving data and Opening Excel file.


        //        if (string.IsNullOrEmpty(PathToSave) == false)
        //            book.SaveAs(PathToSave);

        //        excelApp.Visible = true;

        //        #endregion

        //        #region Release objects

        //        try
        //        {
        //            if (sheet != null)
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
        //            sheet = null;

        //            if (sheets != null)
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(sheets);
        //            sheets = null;

        //            if (book != null)
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
        //            book = null;

        //            if (books != null)
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(books);
        //            books = null;

        //            if (excelApp != null)
        //                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
        //            excelApp = null;
        //        }
        //        catch (Exception ex)
        //        {
        //            sheet = null;
        //            sheets = null;
        //            book = null;
        //            books = null;
        //            excelApp = null;
        //        }
        //        finally
        //        {
        //            GC.Collect();
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    #endregion
        //} 


        public static string SerializeToXml(this object obj)
        {
            XDocument doc = new XDocument();
            using (XmlWriter xmlWriter = doc.CreateWriter())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                xmlSerializer.Serialize(xmlWriter, obj);
                xmlWriter.Close();
            }
            return doc.ToString();
        }

        /// <summary> 
        /// Converts an IEnumerable to a HashSet 
        /// </summary> 
        /// <typeparam name="T">The IEnumerable type</typeparam> 
        /// <param name="enumerable">The IEnumerable</param> 
        /// <returns>A new HashSet</returns> 
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            HashSet<T> hs = new HashSet<T>();
            foreach (T item in enumerable)
                hs.Add(item);
            return hs;
        }


        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;

            yield break;
        }


        /// <summary> 
        /// Delete files in a folder that are like the searchPattern, don't include subfolders. 
        /// </summary> 
        /// <param name="di"></param> 
        /// <param name="searchPattern">DOS like pattern (example: *.xml, ??a.txt)</param> 
        /// <returns>Number of files that have been deleted.</returns> 
        public static int DeleteFiles(this DirectoryInfo di, string searchPattern)
        {
            return DeleteFiles(di, searchPattern, false);
        }

        /// <summary> 
        /// Delete files in a folder that are like the searchPattern 
        /// </summary> 
        /// <param name="di"></param> 
        /// <param name="searchPattern">DOS like pattern (example: *.xml, ??a.txt)</param> 
        /// <param name="includeSubdirs"></param> 
        /// <returns>Number of files that have been deleted.</returns> 
        /// <remarks> 
        /// This function relies on DirectoryInfo.GetFiles() which will first get all the FileInfo objects in memory. This is good for folders with not too many files, otherwise 
        /// an implementation using Windows APIs can be more appropriate. I didn't need this functionality here but if you need it just let me know. 
        /// </remarks> 
        public static int DeleteFiles(this DirectoryInfo di, string searchPattern, bool includeSubdirs)
        {
            int deleted = 0;
            foreach (FileInfo fi in di.GetFiles(searchPattern, includeSubdirs ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                fi.Delete();
                deleted++;
            }

            return deleted;
        }

        public static IEnumerable<T> Select<T>(this SqlDataReader reader, Func<SqlDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }

        /// <summary> 
        /// Sets all values. 
        /// </summary> 
        /// <typeparam name="T">The type of the elements of the array that will be modified.</typeparam> 
        /// <param name="array">The one-dimensional, zero-based array</param> 
        /// <param name="value">The value.</param> 
        /// <returns>A reference to the changed array.</returns> 
        public static T[] SetAllValues<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }

            return array;
        }

        public static Func<TResult> Async<T, TResult>(this IEnumerable<T> enumerable, Func<IEnumerable<T>, TResult> asyncSelector)
        {
            System.Diagnostics.Debug.Assert(!(enumerable is ICollection), "Async does not work on arrays/lists/collections, only on true enumerables/queryables.");

            // Create delegate to exec async 
            Func<IEnumerable<T>, TResult> work = (e => asyncSelector(e));

            // Launch it 
            IAsyncResult r = work.BeginInvoke(enumerable, null, null);

            // Return method that will block until completed and rethrow exceptions if any 
            return () => work.EndInvoke(r);
        }


        //public static string ToCsv<T>(this IEnumerable<T> instance, bool includeColumnHeader, string[] properties)
        //{
        //    if (instance == null)
        //        return null;

        //    var csv = new StringBuilder();

        //    if (includeColumnHeader)
        //    {
        //        var header = new StringBuilder();
        //        foreach (var property in properties)
        //            header.AppendFormat("{0},", property);

        //        csv.AppendLine(header.ToString(0, header.Length - 1));
        //    }

        //    foreach (var item in instance)
        //    {
        //        var row = new StringBuilder();

        //        foreach (var property in properties)
        //            row.AppendFormat("{0},", item.GetPropertyValue<object>(property));

        //        csv.AppendLine(row.ToString(0, row.Length - 1));
        //    }

        //    return csv.ToString();
        //}

        //public static string ToCsv<T>(this IEnumerable<T> instance, bool includeColumnHeader)
        //{
        //    if (instance == null)
        //        return null;

        //    var properties = (from p in typeof(T).GetProperties()
        //                      select p.Name).ToArray();

        //    return ToCsv(instance, includeColumnHeader, properties);
        //}


        //public static bool DataBind(this ListControl control, object datasource, string textField, string valueField)
        //{
        //    return DataBind(control, datasource, textField, null, valueField);
        //}

        //public static bool DataBind(this ListControl control, object datasource, string textField, string textFieldFormat, string valueField)
        //{
        //    control.DataTextField = textField;
        //    control.DataValueField = valueField;

        //    if (!string.IsNullOrEmpty(textFieldFormat))
        //        control.DataTextFormatString = textFieldFormat;

        //    control.DataSource = datasource;
        //    control.DataBind();

        //    if (control.Items.Count > 0)
        //    {
        //        control.SelectedIndex = 0;
        //        return true;
        //    }
        //    else return false;
        //}


        public static string ToXML<T>(this T o) where T : new()
        {
            string retVal;
            using (var ms = new MemoryStream())
            {
                var xs = new XmlSerializer(typeof (T));
                xs.Serialize(ms, o);
                ms.Flush();
                ms.Position = 0;
                var sr = new StreamReader(ms);
                retVal = sr.ReadToEnd();
            }
            return retVal;
        }


        /// <summary> 
        /// Strip a string of the specified character. 
        /// </summary> 
        /// <param name="s">the string to process</param> 
        /// <param name="char">character to remove from the string</param> 
        /// <example> 
        /// string s = "abcde"; 
        ///  
        /// s = s.Strip('b');  //s becomes 'acde; 
        /// </example> 
        /// <returns></returns> 
        public static string Strip(this string s, char character)
        {
            s = s.Replace(character.ToString(), "");

            return s;
        }

        /// <summary> 
        /// Strip a string of the specified characters. 
        /// </summary> 
        /// <param name="s">the string to process</param> 
        /// <param name="chars">list of characters to remove from the string</param> 
        /// <example> 
        /// string s = "abcde"; 
        ///  
        /// s = s.Strip('a', 'd');  //s becomes 'bce; 
        /// </example> 
        /// <returns></returns> 
        public static string Strip(this string s, params char[] chars)
        {
            foreach (char c in chars)
            {
                s = s.Replace(c.ToString(), "");
            }

            return s;
        }
        /// <summary> 
        /// Strip a string of the specified substring. 
        /// </summary> 
        /// <param name="s">the string to process</param> 
        /// <param name="subString">substring to remove</param> 
        /// <example> 
        /// string s = "abcde"; 
        ///  
        /// s = s.Strip("bcd");  //s becomes 'ae; 
        /// </example> 
        /// <returns></returns> 
        public static string Strip(this string s, string subString)
        {
            s = s.Replace(subString, "");

            return s;
        }


        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var r = new Random((int)DateTime.Now.Ticks);
            var shuffledList = list.Select(x => new { Number = r.Next(), Item = x }).OrderBy(x => x.Number).Select(x => x.Item);
            return shuffledList.ToList();
        }


        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            IEnumerable<TThird> third,
            Func<TFirst, TSecond, TThird, TResult> resultSelector)
        {
            Contract.Requires(first != null && second != null && third != null && resultSelector != null);

            using (IEnumerator<TFirst> iterator1 = first.GetEnumerator())
            using (IEnumerator<TSecond> iterator2 = second.GetEnumerator())
            using (IEnumerator<TThird> iterator3 = third.GetEnumerator())
            {
                while (iterator1.MoveNext() && iterator2.MoveNext() && iterator3.MoveNext())
                {
                    yield return resultSelector(iterator1.Current, iterator2.Current, iterator3.Current);
                }
            }
        }


        public static IEnumerable<T> RemoveDuplicates<T>(this ICollection<T> list, Func<T, int> Predicate)
        {
            var dict = new Dictionary<int, T>();
            foreach (var item in list)
            {
                if (!dict.ContainsKey(Predicate(item)))
                {
                    dict.Add(Predicate(item), item);
                }
            }
            return dict.Values.AsEnumerable();
        }



        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) return;
            var cached = items;
            foreach (var item in cached)
                action(item);
        }


        public static CompilerResults CSharpCompile(this string code, string dllName = "dynamicCompile", params string[] additionalAssemblies)
        {
            var csc = new CSharpCodeProvider(new Dictionary<string, string>() {{"CompilerVersion", "v4.0"}});
            var parameters = new CompilerParameters
                {
                    ReferencedAssemblies =
                        {
                            "mscorlib.dll",
                            "System.Core.dll",
                        },
                    OutputAssembly = dllName,
                    GenerateExecutable = false,
                    GenerateInMemory = true,
                };

            parameters.ReferencedAssemblies.AddRange(additionalAssemblies);

            return csc.CompileAssemblyFromSource(parameters, code);
        }

        public static T ConvertJsonStringToObject<T>(this string stringToDeserialize)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<T>(stringToDeserialize);
        }

        public static bool IsStrongPassword(this string s)
        {
            bool isStrong = Regex.IsMatch(s, @"[\d]");
            if (isStrong) isStrong = Regex.IsMatch(s, @"[a-z]");
            if (isStrong) isStrong = Regex.IsMatch(s, @"[A-Z]");
            if (isStrong) isStrong = Regex.IsMatch(s, @"[\s~!@#\$%\^&\*\(\)\{\}\|\[\]\\:;'?,.`+=<>\/]");
            if (isStrong) isStrong = s.Length > 7;
            return isStrong;
        }

        public static bool Between<T>(this T me, T lower, T upper) where T : IComparable<T>
        {
            return me.CompareTo(lower) >= 0 && me.CompareTo(upper) < 0;
        }


        public static void BinarySerializer<T>(this IList<T> lista, string path)
        {
            if (lista == null)
            {
                throw new ArgumentNullException("lista", "variavel de destino não pode ser nula");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path", "caminho do xml não pode ser nulo ou vazio");
            }

            try
            {
                using (Stream stream = File.Open(path, FileMode.Create, FileAccess.Write))
                {
                    var bin = new BinaryFormatter();
                    bin.Serialize(stream, lista);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public static string DeleteChars(this string input, params char[] chars)
        {
            return new string(input.Where((ch) => !chars.Contains(ch)).ToArray());
        }


        public static List<string> ListFiles(this string folderPath)
        {
            if (!Directory.Exists(folderPath)) return null;
            return (from f in Directory.GetFiles(folderPath)
                    select Path.GetFileName(f)).
                ToList();
        }

        /// <summary> 
        /// Kilobytes 
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static int KB(this int value)
        {
            return value * 1024;
        }

        /// <summary> 
        /// Megabytes 
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static int MB(this int value)
        {
            return value.KB() * 1024;
        }

        /// <summary> 
        /// Gigabytes 
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static int GB(this int value)
        {
            return value.MB() * 1024;
        }

        /// <summary> 
        /// Terabytes 
        /// </summary> 
        /// <param name="value"></param> 
        /// <returns></returns> 
        public static long TB(this int value)
        {
            return (long)value.GB() * (long)1024;
        }


        #endregion Methods
    }
}
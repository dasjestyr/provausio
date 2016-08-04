using System;
using System.Data;

namespace Provausio.Data.Ado.Extensions
{
    public static class DataRecordExt
    {
        /// <summary>
        /// Attempts to read the field from the record and cast it to the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record">The record.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="throwOnfailure">if set to <c>true</c> [throw onfailure].</param>
        /// <returns></returns>
        public static T DbCast<T>(this IDataRecord record, string fieldName, bool throwOnfailure = true)
        {
            try
            {
                var obj = record[fieldName];
                var asT = (T)Convert.ChangeType(obj, typeof(T));
                return asT;
            }
            catch (FormatException)
            {
                if (throwOnfailure)
                    throw;

                return default(T);
            }
        }

        /// <summary>
        /// Attempts to read the field from the record and cast it to the specified nullable struct type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="record">The record.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="throwOnFailure">if set to <c>true</c> [throw on failure].</param>
        /// <returns></returns>
        public static T? DbCastNullable<T>(this IDataRecord record, string fieldName, bool throwOnFailure = true)
            where T : struct
        {
            try
            {
                var obj = record[fieldName];
                if (obj == null || obj == DBNull.Value)
                    return null;

                var asT = (T?)obj;
                return asT;
            }
            catch
            {
                if (throwOnFailure)
                    throw;

                return null;
            }
        }
    }
}

﻿using System;
using System.Data;
using System.Data.SqlClient;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite.Converters;

namespace ServiceStack.OrmLite.SqlServer.Converters
{
    public class SqlServerStringConverter : StringConverter
    {
        public override string MaxColumnDefinition => UseUnicode ? "NVARCHAR(MAX)" : "VARCHAR(MAX)";

        public override string GetColumnDefinition(int? stringLength)
        {
            if (stringLength.GetValueOrDefault() == StringLengthAttribute.MaxText)
                return MaxColumnDefinition;

            var safeLength = Math.Min(
                stringLength.GetValueOrDefault(StringLength), 
                UseUnicode ? 4000 : 8000);

            return UseUnicode
                ? "NVARCHAR({0})".Fmt(safeLength)
                : "VARCHAR({0})".Fmt(safeLength);
        }

        public override void InitDbParam(IDbDataParameter p, Type fieldType)
        {
            base.InitDbParam(p, fieldType);

            var sqlParam = p as SqlParameter;
            if (sqlParam == null) return;

            if (!UseUnicode)
            {
                sqlParam.SqlDbType = SqlDbType.VarChar;
            }
        }
    }
}
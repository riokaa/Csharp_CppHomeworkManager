﻿using System;
using System.Data.SQLite;

namespace EmailHomeworkSystem.Database {
    class SqLiteHelper {

        private SQLiteDataReader dataReader;           //数据读取定义
        private SQLiteCommand dbCommand;                //SQL命令定义
        private SQLiteConnection dbConnection;         //数据库连接定义

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connectionString"></param>
        public SqLiteHelper(string connectionString) {
            try {
                dbConnection = new SQLiteConnection("data source=" + connectionString + ";UseUTF16Encoding=True;");
                dbConnection.Open();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 新建数据库文件
        /// </summary>
        /// <param name="dbPath"></param>
        /// <returns></returns>
        public static Boolean NewDbFile(string dbPath) {
            try {
                SQLiteConnection.CreateFile(dbPath);
                return true;
            } catch (Exception ex) {

                throw new Exception("新建数据库文件" + dbPath + "失败：" + ex.Message);
            }

        }


        /// <summary>
        /// 执行SQl命令
        /// </summary>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public SQLiteDataReader ExecuteQuery(string queryString) {

            try {
                dbCommand = dbConnection.CreateCommand();
                dbCommand.CommandText = queryString;
                dataReader = dbCommand.ExecuteReader();
            } catch (Exception e) {
                Console.WriteLine(e.Message);

            }

            return dataReader;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseConnection() {
            //销毁Commend
            if (dbCommand != null) {
                dbCommand.Cancel();
            }
            dbCommand = null;

            //销毁Reader
            if (dataReader != null) {
                dataReader.Close();
            }
            dataReader = null;

            //销毁Connection
            if (dbConnection != null) {
                dbConnection.Close();
            }
            dbConnection = null;

        }


        /// <summary>
        /// 读取整张数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public SQLiteDataReader ReadFullTable(string tableName) {
            string queryString = "SELECT * FROM " + tableName;

            return ExecuteQuery(queryString);
        }


        /// <summary>
        /// 向指定数据表中插入数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="values">数值</param>
        /// <returns></returns>
        public SQLiteDataReader InsertValues(string tableName, string[] values) {
            //获取数据表中字段数目
            int fieldCount = ReadFullTable(tableName).FieldCount;

            if (values.Length != fieldCount) {
                throw new SQLiteException("Values.Length!=fieldCount");
            }

            string queryString = "INSERT INTO " + tableName + " VALUES (" + "'" + values[0] + "'";

            for (int i = 1; i < values.Length; i++) {
                queryString += ", " + "'" + values[i] + "'";

            }
            queryString += ")";
            return ExecuteQuery(queryString);

        }


        /// <summary>
        /// 更新指定数据表内的数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="colNames"></param>
        /// <param name="colValues"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public SQLiteDataReader UpdateValues(string tableName, string[] colNames, string[] colValues, string key, string value, string operation = "=") {

            //当字段名称和字段数值不对应时引发异常
            if (colNames.Length != colValues.Length) {
                throw new SQLiteException("colNames.Length!=colValues.Leght");

            }
            string queryString = "UPDATE" + tableName + " SET " + colNames[0] + "=" + "'" + colValues[0] + "'";

            for (int i = 0; i < colValues.Length; i++) {
                queryString += ", " + colNames[i] + "=" + "'" + colValues[i] + "'";
            }
            queryString += "WHWERE " + key + operation + "'" + value + "'";
            return ExecuteQuery(queryString);


        }

        /// <summary>
        /// 删除指定数据表内的数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="colNames"></param>
        /// <param name="colValues"></param>
        /// <param name="operations"></param>
        /// <returns></returns>
        public SQLiteDataReader DeleteValuesOR(string tableName, string[] colNames, string[] colValues, string[] operations) {
            //当字段名称和字段数值不对应时引发异常
            if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length) {
                throw new SQLiteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
            }

            string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + "'" + colValues[0] + "'";

            for (int i = 0; i < colValues.Length; i++) {
                queryString += "OR" + colValues[i] + operations[0] + "'" + colValues[i] + "'";
            }

            return ExecuteQuery(queryString);

        }


        /// <summary>
        /// 删除指定数据表内的数据
        /// </summary>
        /// <returns>The values.</returns>
        /// <param name="tableName">数据表名称</param>
        /// <param name="colNames">字段名</param>
        /// <param name="colValues">字段名对应的数据</param>
        public SQLiteDataReader DeleteValuesAND(string tableName, string[] colNames, string[] colValues, string[] operations) {
            //当字段名称和字段数值不对应时引发异常
            if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length) {
                throw new SQLiteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
            }

            string queryString = "DELETE FROM " + tableName + " WHERE " + colNames[0] + operations[0] + "'" + colValues[0] + "'";
            for (int i = 1; i < colValues.Length; i++) {
                queryString += " AND " + colNames[i] + operations[i] + "'" + colValues[i] + "'";
            }
            return ExecuteQuery(queryString);
        }



        /// <summary>
        /// 创建数据表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="colNames"></param>
        /// <param name="colTypes"></param>
        /// <returns></returns>
        public SQLiteDataReader CreateTable(string tableName, string[] colNames, string[] colTypes) {
            string queryString = "CREATE TABLE IF NOT EXISTS " + tableName + "( " + colNames[0] + " " + colTypes[0];
            for (int i = 1; i < colNames.Length; i++) {
                queryString += ", " + colNames[i] + " " + colTypes[i];
            }
            queryString += "  ) ";
            return ExecuteQuery(queryString);
        }

        /// <summary>
        /// Reads the table.
        /// </summary>
        /// <returns>The table.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="items">Items.</param>
        /// <param name="colNames">Col names.</param>
        /// <param name="operations">Operations.</param>
        /// <param name="colValues">Col values.</param>
        public SQLiteDataReader ReadTable(string tableName, string[] items, string[] colNames, string[] operations, string[] colValues) {
            string queryString = "SELECT " + items[0];
            for (int i = 1; i < items.Length; i++) {
                queryString += ", " + items[i];
            }
            queryString += " FROM " + tableName + " WHERE " + colNames[0] + " " + operations[0] + " " + colValues[0];
            for (int i = 0; i < colNames.Length; i++) {
                queryString += " AND " + colNames[i] + " " + operations[i] + " " + colValues[0] + " ";
            }
            return ExecuteQuery(queryString);
        }


    }
}
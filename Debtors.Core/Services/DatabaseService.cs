using Debtors.Core.Helpers;
using Debtors.Core.Interfaces;
using Debtors.Core.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Debtors.Core.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly SQLiteConnection Connection;
        private static object collisionLock;
        public DatabaseService()
        {
            collisionLock = new object();
            Connection = new SQLiteConnection(StaticHelper.DatabasePath);
            CreateTables();
        }

        private void CreateTables()
        {
            Connection.CreateTable<Phone>();
            Connection.CreateTable<Mail>();
            Connection.CreateTable<Debt>();
            Connection.CreateTable<Debtor>();
        }

        public List<Debtor> GetDebtors()
        {
            List<Debtor> toReturn = new List<Debtor>();
            lock (collisionLock)
            {
                try
                {
                    toReturn = Connection.Table<Debtor>().ToList();
                }
                catch (Exception ex)
                {

                }
            }
            return toReturn;
        }

        public Debtor GetDebtor(int id)
        {
            Debtor toReturn = new Debtor();
            lock (collisionLock)
            {
                try
                {
                    toReturn = Connection.Get<Debtor>(id);
                }
                catch (Exception ex)
                {

                }
            }
            return toReturn;
        }

        public int InsertOrUpdateDebtor(Debtor debtor)
        {
            int toReturn = 0;
            lock (collisionLock)
            {
                try
                {
                    Connection.BeginTransaction();

                    if (debtor.Id > 0)
                        toReturn = Connection.Update(debtor);
                    else
                        toReturn = Connection.Insert(debtor);
                    
                    toReturn = Connection.InsertOrReplace(debtor);
                    Connection.Commit();
                }
                catch (Exception ex)
                {
                    if (Connection.IsInTransaction)
                        Connection.Rollback();
                }
            }
            return toReturn;
        }

        public int RemoveDebtor(Debtor debtor)
        {
            int toReturn = 0;
            lock (collisionLock)
            {
                try
                {
                    Connection.BeginTransaction();
                    toReturn = Connection.Delete(debtor);
                    Connection.Commit();
                }
                catch (Exception ex)
                {
                    if (Connection.IsInTransaction)
                        Connection.Rollback();
                }
            }
            return toReturn;
        }

        public int RemoveDebtor(int id)
        {
            int toReturn = 0;
            lock (collisionLock)
            {
                try
                {
                    Connection.BeginTransaction();
                    toReturn = Connection.Delete<Debtor>(id);
                    Connection.Commit();
                }
                catch (Exception ex)
                {
                    if (Connection.IsInTransaction)
                        Connection.Rollback();
                }
            }
            return toReturn;
        }
    }
}

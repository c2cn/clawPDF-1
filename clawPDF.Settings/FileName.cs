using System;
using System.Collections.Generic;
using Microsoft.Win32;
using NLog;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;

namespace clawSoft.clawPDF.Core.Settings
{
    public class FileName
    {
        private const string clawPDF_REG_PATH = @"Software\clawSoft\clawPDF";
        private const string FILENMAE_REG_PATH = clawPDF_REG_PATH + @"\FileNames";
        private static Data data;
        private static IStorage storage;
        public static List<FileInfo>  Files;
        public const string StartTime = "StartTime";
        public const string EndTime = "EndTime";
        public const string PrintState = "PrintState";
        public const string JobId = "JobId";
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();


        public static void Init()
        {
            data = Data.CreateDataStorage();
            storage = new RegistryStorage(RegistryHive.CurrentUser, FILENMAE_REG_PATH);
            Files = new List<FileInfo>();
            ReadValues();
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        public static void ReadValues()
        {
            try
            {
                data.Clear();
                Files.Clear();
                string[] filenames = { };
                storage.SetData(data);
                storage.ReadData("");
                filenames = data.GetSections();
                if (null != filenames && filenames.Length > 0)
                {
                    foreach (string filename in filenames)
                    {
                        FileInfo temp = new FileInfo();
                        string name = filename.Replace("\\", "");
                        temp.Name       = name;
                        temp.JobId      = getValueByFileName(name, JobId);
                        temp.StartTime  = getValueByFileName(name, StartTime);
                        temp.EndTime    = getValueByFileName(name, EndTime);
                        temp.PrintState = getValueByFileName(name, PrintState);
                        Logger.Debug("��ȡע���Ĵ�ӡ�ļ���Ϣ" + temp.ToString());
                        Files.Add(temp);
                    }
                }
            }
            catch
            {
                data.Clear();
                storage.SetData(data);
                storage.WriteData(); 
            }
        }

        /// <summary>
        /// �������ƻ�ȡ����ֵ
        /// </summary>
        /// <param name="filename">�ļ���</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public static bool getBoolValueByFileName(string filename, string name)
        {
            try
            {
                string value = data.GetValue(@"" + filename + @"" + name);
                if (null != value)
                {
                    return bool.Parse(value);
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// �������ƻ�ȡֵ
        /// </summary>
        /// <param name="path">ע���·��</param>
        /// <param name="name">������</param>
        /// <returns></returns>
        public static string getValueByFileName(string filename, string name)
        {
            try
            {
                return data.GetValue(@"" + filename + "\\"+ @"" + name);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        ///  �����ļ����޸��ļ���Ϣ
        /// </summary>
        /// <param name="filename">�ļ���</param>
        /// <param name="name">������</param>
        /// <param name="value">�趨ֵ</param>
        public static void modifyFileInfoByFileName(string filename, string name, string value)
        {
            data.SetValue(@"" + filename + "\\" + @"" + name, value);
            storage.SetData(data);
            storage.WriteData();
        }

        /// <summary>
        /// �޸�ע�����ļ���Ϣ
        /// </summary>
        /// <param name="fileInfo"></param>
        public static void modifyFileInfo(FileInfo fileInfo)
        {
           data.SetValue(@"" + fileInfo.Name + "\\" + @"" + FileName.StartTime, fileInfo.StartTime);
           data.SetValue(@"" + fileInfo.Name + "\\" + @"" + FileName.EndTime, fileInfo.EndTime);
           data.SetValue(@"" + fileInfo.Name + "\\" + @"" + FileName.PrintState, fileInfo.PrintState);
           data.SetValue(@"" + fileInfo.Name + "\\" + @"" + FileName.JobId, fileInfo.JobId);
           storage.SetData(data);
           storage.WriteData();
        }

        /// <summary>
        /// ��ȡδ��ӡ���ļ�
        /// </summary>
        /// <returns></returns>
        public static FileInfo getFileInfoNoPrint()
        {
            Logger.Debug("ƥ���ļ��б���"   +  Files.Count);
            Logger.Debug("ƥ���ļ��б�����" +  Files.ToString());
            foreach (FileInfo file in Files)
            {
                Logger.Debug("��ƥ��Ĵ�ӡ�ļ���Ϣ" + file.ToString());
                if (string.IsNullOrEmpty(file.PrintState) || file.PrintState == "0")
                {
                    return file;
                }
            }
            return null;
        }

        public static FileInfo getFileInfoByName(string name)
        {
            foreach (FileInfo file in Files)
            {
                if (file.Name.Equals(name))
                {
                    return file;
                }
            }
            return null;
        }
    }
}
using System;

namespace AuxiliaryLibrary
{
    /// <summary>
    /// Представляет информацию о потоке данных
    /// </summary>
    [Serializable]
    public class StreamInfo
    {
        /// <param name="fullSize">Полный объем потока</param>
        /// <param name="isFile">Является ли поток файлом</param>
        /// <param name="fileName">Имя файла, если является им</param>
        /// <param name="fileExt">Расширение файла файла, если является им</param>
        public StreamInfo(long fullSize, bool isFile = false, string fileName = null, string fileExt = null)
        {
            if (fullSize <= 0) throw new ArgumentException("Wrong size");

            FullSize = fullSize;

            IsFile = isFile;

            FileName = fileName;
            FileExt = fileExt;
        }
        /// <summary>
        /// Полный объем потока
        /// </summary>
        public readonly long FullSize;

        /// <summary>
        /// Является ли поток файлом
        /// </summary>
        public readonly bool IsFile;
        
        string fileName;
        /// <summary>
        /// Имя файла, если является им
        /// </summary>
        public string FileName
        {
            get
            {
                if (IsFile) return fileName;
                else throw new NotSupportedException();
            }
            protected set
            { fileName = value; }
        }
        
        string fileExt;
        /// <summary>
        /// Расширение файла файла, если является им
        /// </summary>
        public string FileExt
        {
            get
            {
                if (IsFile) return fileExt;
                else throw new NotSupportedException();
            }
            protected set
            { fileExt = value; }
        }
    }
}
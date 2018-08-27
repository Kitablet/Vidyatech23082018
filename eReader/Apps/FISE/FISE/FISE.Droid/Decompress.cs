using System;
using Android.Util; 
using Java.IO;
using Java.Util.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace Kitablet.Droid
{
	public class Decompress
	{
		String _zipFile; 
		String _location; 

		public Decompress(String zipFile, String location)
		{ 
			_zipFile = zipFile; 
			_location = location;             
			DirChecker(""); 
		}

		void DirChecker(String dir)
		{ 
			var file = new File(_location + dir); 

			if(!file.IsDirectory) { 
				file.Mkdirs ();
			} 
		}
        public void UnZip1()
        {
            try
            {
                var zipInputStream = new ZipInputStream(System.IO.File.OpenRead(_zipFile));
                ZipEntry zipEntry = null;

                while ((zipEntry = zipInputStream.NextEntry) != null)
                {
                    if (zipEntry.IsDirectory)
                    {
                        DirChecker(zipEntry.Name);
                    }
                    else
                    {
                        string dirName = System.IO.Path.GetDirectoryName(_location + zipEntry.Name);
                        if (!System.IO.Directory.Exists(System.IO.Path.GetFullPath(dirName)))
                        {
                            System.IO.Directory.CreateDirectory(System.IO.Path.GetFullPath(dirName));
                        }

                        var fileOutputStream = new FileOutputStream(_location + zipEntry.Name);


                        //if (!zipEntry.Name.Contains(".ttf"))
                        {
                            for (int i = zipInputStream.Read(); i != -1; i = zipInputStream.Read())
                            {
                                fileOutputStream.Write(i);
                            }
                        }

                        zipInputStream.CloseEntry();
                        fileOutputStream.Close();
                    }
                }
                zipInputStream.Close();
            }
            catch (Exception ex)
            {
                Log.Error("Decompress", "UnZip", ex);
            }
        }



        public void UnZip()
        {
            ICSharpCode.SharpZipLib.Zip.ZipFile zf = null;
            try
            {
                System.IO.FileStream fs = System.IO.File.OpenRead(_zipFile);
                zf = new ICSharpCode.SharpZipLib.Zip.ZipFile(fs);
                foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry zipEntry in zf)
                {
                    if (zipEntry.IsDirectory)
                    {
                        DirChecker(zipEntry.Name);          // Ignore directories
                    }
                    else
                    { 
                        String entryFileName = zipEntry.Name;
                        // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                        // Optionally match entrynames against a selection list here to skip as desired.
                        // The unpacked length is available in the zipEntry.Size property.

                        byte[] buffer = new byte[4096];     // 4K is optimum
                        System.IO.Stream zipStream = zf.GetInputStream(zipEntry);

                        // Manipulate the output filename here as desired.
                        string directoryName = System.IO.Path.GetDirectoryName(_location + zipEntry.Name);
                        if (directoryName.Length > 0)
                            System.IO.Directory.CreateDirectory(directoryName);

                        // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                        // of the file, but does not waste memory.
                        // The "using" will close the stream even if an exception occurs.
                        using (System.IO.FileStream streamWriter = System.IO.File.Create(_location + zipEntry.Name))
                        {
                            StreamUtils.Copy(zipStream, streamWriter, buffer);
                        }
                    }
                }
            }
            finally
            {
                if (zf != null)
                {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }

    }
}
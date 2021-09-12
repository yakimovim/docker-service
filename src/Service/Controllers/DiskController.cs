﻿using Microsoft.AspNetCore.Mvc;
using Service.Controllers.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Service.Controllers
{
    public class DiskController : Controller
    {
        [MainMenuElement("Disk")]
        [HttpGet]
        public IActionResult Index()
        {
            var fileSystemObjects = new LinkedList<FileSystemObject>();

            if(Request.Query.ContainsKey("path"))
            {
                var path = HttpUtility.UrlDecode(Request.Query["path"]);

                var directoryInfo = new DirectoryInfo(path);

                var parentDirectory = directoryInfo.Parent;

                if(parentDirectory != null)
                {
                    fileSystemObjects.AddLast(new FileSystemObject(
                        FileSystemObjectType.Directory,
                        "..",
                        HttpUtility.UrlEncode(parentDirectory.FullName)
                    ));
                }
                else
                {
                    fileSystemObjects.AddLast(new FileSystemObject(
                        FileSystemObjectType.Directory,
                        "..",
                        string.Empty
                    ));
                }

                foreach (var subDirectory in directoryInfo.GetDirectories())
                {
                    fileSystemObjects.AddLast(new FileSystemObject(
                        FileSystemObjectType.Directory,
                        subDirectory.Name,
                        HttpUtility.UrlEncode(subDirectory.FullName)
                    ));
                }

                foreach (var files in directoryInfo.GetFiles())
                {
                    fileSystemObjects.AddLast(new FileSystemObject(
                        FileSystemObjectType.File,
                        files.Name,
                        HttpUtility.UrlEncode(files.FullName)
                    ));
                }
            }
            else
            {
                var drives = DriveInfo.GetDrives();

                foreach(var drive in drives)
                {
                    var rootDirectory = drive.RootDirectory;

                    fileSystemObjects.AddLast(new FileSystemObject(
                        FileSystemObjectType.Directory,
                        rootDirectory.Name,
                        HttpUtility.UrlEncode(rootDirectory.FullName)
                    ));
                }
            }

            return View(fileSystemObjects);
        }
    }

    public enum FileSystemObjectType
    {
        Directory,
        File
    }

    public sealed class FileSystemObject
    {
        public FileSystemObject(FileSystemObjectType type, string name, string path)
        {
            Type = type;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public FileSystemObjectType Type { get; }

        public string Name { get; }

        public string Path { get; }
    }
}

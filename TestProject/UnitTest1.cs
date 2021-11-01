using System.Collections.Generic;
using System.IO;
using CopyToLocale.Core.Models;
using CopyToLocales.Xaml;
using CopyToLocates.Resx;
using NUnit.Framework;

namespace TestProject
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void XamlFormatManager()
        {
            var path = @"Z:\Repositories\GitHub\CopyResx\CopyToLocales\bin\Debug\net5.0-windows\TestFiles\Xaml\StringResources.xaml";
            bool exist = File.Exists(path);
            var ls = new List<SelectFileModel>()
            {
                new SelectFileModel()
                {
                    SourceKey = ".ru.resx",
                    FullPath = path
                }
            };
            var test = new XamlFormatManager();
            test.InitReaders(ls);
        }
        
        [Test]
        public void ResxFormatManager()
        {
            var path = @"Z:\Repositories\GitHub\CopyResx\CopyToLocales\bin\Debug\net5.0-windows\TestFiles\Resx\LocalizationStrings.resx";
            bool exist = File.Exists(path);
            var ls = new List<SelectFileModel>()
            {
                new SelectFileModel()
                {
                    SourceKey = ".ru.resx",
                    FullPath = path
                }
            };
            var test = new ResxFormatManager();
            test.InitReaders(ls);
        }
    }
}
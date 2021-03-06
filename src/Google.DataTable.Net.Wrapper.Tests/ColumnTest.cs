﻿/*
   Copyright 2012 Zoran Maksimovic (zoran.maksimovich@gmail.com 
   http://www.agile-code.com)

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Google.DataTable.Net.Wrapper.Tests
{
    [TestFixture]
    public class ColumnTest
    {
        [Test(Description = "Checks that the column object can be created with the default constructor")]
        public void CanInstantiateColumn()
        {
            var c = new Column(ColumnType.String);
            Assert.That(c != null);
        }

        [Test(Description = "Checks that the Role field get's retruend in the json string")]
        public void RoleGetsProperlySerialized()
        {
            //Arrange ------------------
            string columnJson = null;

            var column = new Column(ColumnType.String)
                {
                    Role = ColumnRole.Annotation
                };

            //Act ----------------------
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();

                StreamWriter sw = new StreamWriter(ms);

                column.GetJson(sw);

                sw.Flush();
                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                {
                    columnJson = sr.ReadToEnd();
                }
            }
            catch (System.Exception)
            {

                if (ms != null)
                    ms.Dispose();
            }
            

            //Assert -------------------
            Assert.That(columnJson != null);

            //check the values
            var dictionary = JsonHelper.GetDictionaryFromJson(columnJson);
            var pDictionary = (Dictionary<string,object>)dictionary["p"];
            Assert.That(pDictionary != null);
            Assert.That(pDictionary["role"] != null);

            object roleValue;
            pDictionary.TryGetValue("role", out roleValue);
            Assert.That(((string) roleValue) == ColumnRole.Annotation);            
        }

        [Test(Description = "If the property and role are specified, this tests that the output in json gets generated properly")]
        public void Column_Property_And_Role_Specified()
        {
            //Arrange ------------------
            string columnJson;

            var column = new Column(ColumnType.String);
            column.Role = ColumnRole.Annotation;
            column.AddProperty(new Property("property1", "value"));

            //Act ----------------------
            using (var ms = new MemoryStream())
            {
                var sw = new StreamWriter(ms);

                column.GetJson(sw);

                sw.Flush();
                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                {
                    columnJson = sr.ReadToEnd();
                }
            }

            //Assert -------------------
            Assert.That(columnJson != null);

            var dictionary = JsonHelper.GetDictionaryFromJson(columnJson);

            object roleValue;
            object propValue;

            var pDictionary = (Dictionary<string, object>)dictionary["p"];
            pDictionary.TryGetValue("role", out roleValue);
            pDictionary.TryGetValue("property1", out propValue);

            Assert.That(pDictionary != null);
            Assert.That(pDictionary["role"] != null);
            Assert.That(((string)roleValue) == ColumnRole.Annotation);
            Assert.That(((string)propValue) == "value");
        }
    }
}

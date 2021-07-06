using Bogus;
using EdgeSharp.Core.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace EdgeSharp.Tests.EdgeSharpCore.Infrastructure
{
    public class AppSettingsTests
    {
        [Fact]
        public void WriteReadTest()
        {
            var faker = new Faker("en");

            var expectedItem1 = faker.Lorem.Sentence(5);
            var expectedItem2 = faker.Lorem.Sentence(5);
            var expectedItem3 = faker.Lorem.Sentence(5);
            var expectedItem4 = faker.Lorem.Sentence(5);
            var expectedItem5 = faker.Lorem.Sentence(5);

            AppUser.App.Properties.Settings.Item1 = expectedItem1;
            AppUser.App.Properties.Settings.Item2 = expectedItem2;

            var list = new List<string>();
            list.Add(expectedItem3);
            list.Add(expectedItem4);
            list.Add(expectedItem5);

            AppUser.App.Properties.Settings.TestItems = list;
      
            // Delete config file if exists
            DeleteConfigFile();

            // Save settings
            var config = new Core.Defaults.Configuration();
            AppUser.App.Properties.Save(config);

            // Read
            AppUser.App.Properties.Settings.Item1 = null;
            AppUser.App.Properties.Settings.Item2 = null;
            AppUser.App.Properties.Settings.TestItems = null;
            AppUser.App.Properties.Read(config);

            var actualItem1 = (string)AppUser.App.Properties.Settings.Item1;
            var actualItem2 = (string)AppUser.App.Properties.Settings.Item2;
            var actualTestItems = (ArrayList)AppUser.App.Properties.Settings.TestItems;

            Assert.NotNull(actualItem1);
            Assert.NotNull(actualItem2);
            Assert.NotNull(actualTestItems);

            Assert.Equal(expectedItem1, actualItem1);
            Assert.Equal(expectedItem2, actualItem2);
            Assert.Equal(expectedItem3, actualTestItems[0]);
            Assert.Equal(expectedItem4, actualTestItems[1]);
            Assert.Equal(expectedItem5, actualTestItems[2]);

            // Delete config file if exists
            DeleteConfigFile();
        }

        private void DeleteConfigFile()
        {
            try
            {
                var appSettingsFile = AppSettingInfo.GetSettingsFilePath();
                if (!string.IsNullOrWhiteSpace(appSettingsFile))
                {
                    if (File.Exists(appSettingsFile))
                    {
                        File.Delete(appSettingsFile);
                    }
                }
            }
            catch { }
        }

    }
}

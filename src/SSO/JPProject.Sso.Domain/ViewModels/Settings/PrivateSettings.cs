﻿using System.Linq;

namespace JPProject.Sso.Domain.ViewModels.Settings
{
    public class PrivateSettings
    {
        public PrivateSettings(Settings settings)
        {
            if (!settings.Any())
                return;

            Smtp = new Smtp(settings["Smtp:Server"], settings["Smtp:Port"], settings["Smtp:UseSsl"], settings["Smtp:Password"], settings["Smtp:Username"]);
            Storage = new StorageSettings(
                                        settings["Storage:Username"],
                                        settings["Storage:Password"],
                                        settings["Storage:Service"],
                                        settings["Storage:StorageName"],
                                        settings["Storage:PhysicalPath"],
                                        settings["Storage:VirtualPath"],
                                        settings["Storage:BasePath"],
                                        settings["Storage:Region"]);

            Recaptcha = new RecaptchaSettings(settings["Recaptcha:SiteKey"], settings["Recaptcha:PrivateKey"]);

            if (bool.TryParse(settings["SendEmail"], out _))
                SendEmail = bool.Parse(settings["SendEmail"]);

            if (bool.TryParse(settings["UseStorage"], out _))
                UseStorage = bool.Parse(settings["UseStorage"]);

            if (bool.TryParse(settings["UseRecaptcha"], out _))
                UseRecaptcha = bool.Parse(settings["UseRecaptcha"]);

        }

        public bool UseRecaptcha { get; set; }
        public bool UseStorage { get; }
        public bool SendEmail { get; }

        public Smtp Smtp { get; }
        public StorageSettings Storage { get; }
        public RecaptchaSettings Recaptcha { get; }
    }
}
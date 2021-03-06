﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigiRazor.PasswordValidation.Factories;
using DigiRazor.PasswordValidation.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DigiRazor.PasswordValidation.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PasswordServiceTest
    {
        private IPassword validTestPassword;

        private IValidatorFactory factory;
        private PasswordRules validTestRules;

        private IPasswordService service;

        [TestInitialize]
        public void Initialise()
        {
            validTestPassword = new Password
            {
                UserId = "ABHW089",
                OldPassword = "B1ge@rs*",
                NewPassword = "yVHn6?R@",
                ConfirmPassword = "yVHn6?R@",
                NewPasswordHash = "yVHn6?R@",

                IsValid = true
            };

            validTestPassword.SetHistory(new List<string> { "$sG96r#X", "3g9m&9W7" });

            validTestRules = new PasswordRules
            {
                Validators = ValidatorTypes.All,
                MinLength = 8,
                MaxLength = 10,
                SpecialChars = new[] { '!', '@', '#', '$', '%', '*', '+', '/' },
                MinHistory = 3,
                Blacklist = new[] { "test", "password" }
            };

            factory = new ValidatorFactory();

            service = new PasswordService(factory);
        }

        [TestCategory("Unit")]
        [TestCategory("Unit-PasswordService")]
        [TestMethod]
        public void Test_PasswordService_No_Injected_Factory()
        {
            var serv = new PasswordService();
            serv.SetupRules(validTestRules);

            var ruleSet = serv.RuleSet;
            ruleSet.Should().NotBeNull();

            var result = serv.Validate(validTestPassword);

            result.Should().NotBe(null);
            result.IsValid.Should().BeTrue();
        }

        [TestCategory("Unit")]
        [TestCategory("Unit-PasswordService")]
        [TestMethod]
        public void Test_PasswordService_SetupRules()
        {
            service.SetupRules(validTestRules);

            var result = service.RuleSet;

            result.Should().NotBeNull();
        }

        [TestCategory("Unit")]
        [TestCategory("Unit-PasswordService")]
        [TestMethod]
        public void Test_PasswordService_SetupRules_Null_Parameter()
        {

            var result = new Action(() =>
            {
                service.SetupRules(null);

            });

            result.ShouldThrow<ArgumentNullException>();
        }

        [TestCategory("Unit")]
        [TestCategory("Unit-PasswordService")]
        [TestMethod]
        public void Test_PasswordService_Validate()
        {
            service.SetupRules(validTestRules);

            var result = service.Validate(validTestPassword);

            result.Should().NotBe(null);
            result.IsValid.Should().BeTrue();
        }

        [TestCategory("Unit")]
        [TestCategory("Unit-PasswordService")]
        [TestMethod]
        public void Test_PasswordService_Validate_Null_Parameter()
        {
            service.SetupRules(validTestRules);

            var result = new Action(() =>
            {
                service.Validate(null);

            });

            result.ShouldThrow<ArgumentNullException>();
        }

        [TestCategory("Unit")]
        [TestCategory("Unit-PasswordService")]
        [TestMethod]
        public void Test_PasswordService_Fail_On_ConfirmPassword()
        {
            var testPassword = new Password
            {
                UserId = "ABHW089",
                OldPassword = "B1ge@rs*",
                NewPassword = "yVHn6?R@",
                ConfirmPassword = "yVHn6?R2",                
                NewPasswordHash = "yVHn6?R@",

                IsValid = true
            };

            testPassword.SetHistory(new List<string> { "$sG96r#X", "3g9m&9W7" });

            service.SetupRules(validTestRules);

            var result = service.Validate(testPassword);

            result.Should().NotBe(null);
            result.IsValid.Should().BeFalse();
        }

        [TestCategory("Unit")]
        [TestCategory("Unit-PasswordService")]
        [TestMethod]
        public void Test_PasswordService_AddCustomValidator()
        {
            service.SetupRules(validTestRules);

            var result = new Action(() =>
            {
                service.AddCustomValidator(new CustomValidator1());

            });

            result.ShouldNotThrow<ArgumentException>();
        }

        [TestCategory("Unit")]
        [TestCategory("Unit-PasswordService")]
        [TestMethod]
        public void Test_PasswordService_AddCustomValidator_Null_Parameter()
        {
            service.SetupRules(validTestRules);

            var result = new Action(() =>
            {
                service.AddCustomValidator(null);

            });

            result.ShouldThrow<ArgumentNullException>();
        }

        [TestCategory("Unit")]
        [TestCategory("Unit-PasswordService")]
        [TestMethod]
        public void Test_PasswordService_AddCustomValidator_InValid()
        {
            service.SetupRules(validTestRules);

            var result = new Action(() =>
            {
                service.AddCustomValidator(new CustomValidator2());

            });

            result.ShouldThrow<ArgumentException>();
        }

    }

    [ExcludeFromCodeCoverage]
    public class CustomValidator1 : IPasswordValidator
    {

        public ValidatorTypes Type
        {
            get { return ValidatorTypes.Custom; }
        }

        public void Setup(PasswordRules ruleSet)
        {

        }

        public IPassword Validate(IPassword value)
        {
            value.IsValid = true;

            return value;
        }

        public string ToString(PasswordRules ruleSet)
        {
            return "Custom Validator 1";
        }
    }

    [ExcludeFromCodeCoverage]
    public class CustomValidator2 : IPasswordValidator
    {

        public ValidatorTypes Type
        {
            get { return ValidatorTypes.Blacklist; }
        }

        public void Setup(PasswordRules ruleSet)
        {

        }

        public IPassword Validate(IPassword value)
        {
            value.IsValid = true;

            return value;
        }

        public string ToString(PasswordRules ruleSet)
        {
            return "Custom Validator 2";
        }
    }
}

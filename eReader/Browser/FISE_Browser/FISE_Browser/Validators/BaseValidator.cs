using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;

namespace FISE_Browser.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
    {
        public BaseValidator()
        {
            PostInitialize();
        }

        protected virtual void PostInitialize()
        {

        }
    }
}
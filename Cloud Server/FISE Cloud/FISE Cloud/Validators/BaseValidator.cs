using FluentValidation;

namespace FISE_Cloud.Validators
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
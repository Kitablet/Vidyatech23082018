using FISE_Cloud.Models.School;
using FluentValidation;

namespace FISE_Cloud.Validators.School
{
    public class EditBookValidator : BaseValidator<BooksListResult>
    {
        public EditBookValidator()
        {
            RuleFor(x => x.Book.Title).NotEmpty().WithMessage(Resource.EditBook_TitleReqError)
                .Length(1, 255).WithMessage(Resource.Book_TitleLength); ;
            RuleFor(x => x.Book.Author).NotEmpty().WithMessage(Resource.EditBook_AuthorReqError)
                .Length(1, 255).WithMessage(Resource.Book_AuthorLength); ;
            RuleFor(x => x.Book.Genre).NotEmpty().WithMessage(Resource.EditBook_CategoryReqerror);
            RuleFor(x => x.Book.Language).NotEmpty().WithMessage(Resource.EditBook_LanguageReqerror);
            RuleFor(x => x.Book.Type).NotEmpty().WithMessage(Resource.EditBook_BookTypeReqerror);
            RuleFor(x => x.Book.SubSection).NotEmpty().WithMessage(Resource.EditBook_SubsectionReqerror);
            RuleFor(x => x.Book.Publisher).NotEmpty().WithMessage(Resource.EditBook_PublisherReqerror)
                .Length(1, 255).WithMessage(Resource.Book_PublisherLength); 
            RuleFor(x => x.Book.Illustrator)
                .Length(0, 255).WithMessage(Resource.Book_IllustratorLength); ;
            RuleFor(x => x.Book.Translator).Length(0,255).WithMessage(Resource.Book_TranslatorLength);              
            RuleFor(x => x.Book.ShortDescription).NotEmpty().WithMessage(Resource.EditBook_ShortDescriptionReqerror)
                .Length(1,1000).WithMessage(Resource.Book_ShorDesc); 
        }

    }
}
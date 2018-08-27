namespace FISE_API
{
    public enum UserStatus { Error = 0, Sucess = 1, UserAlreadyRegistered = 2, UserAccountNotExist = 3, WrongCredentails = 4, InvalidToken = 5, AccountIsNotActive = 6, AccountIsDisabled = 7, LoginIsNotAllowed = 8, SubscriptionExpired = 9,MoreInfo=10 }
    public enum SchoolStatus { Error = 0, Sucess = 1, AccountNotFound = 2, AlreadyHaveSchool = 3, NoSchoolFound = 4, AlreadyHaveSchoolUId = 5 }

    public enum ClassStatus { Error = 0, Sucess = 1, ClassAlreadyExist = 2, NoClassFound = 3, NoSchoolFound = 4 }
    public enum GenericStatus { Error = 0, Sucess = 1, Other = 3 }
    public enum APIStatus { Sucess = 1, NoLiccenseAttached = 2, LicenseExpried = 3, InvalidLicenseKey = 4, LicenseAlreadyAssigned = 5, UserNotRegistered = 6, UserAlreadyRegisted = 7, Error = 8, LicenseLanguageMisMatch = 9, UserAccountNotExist = 10, WrongCredentails = 11, SuccessfulLoginWithDemoLicense = 12, SyncPeriodOver = 13, DuplicateProductRegistration = 14, InvalidUserRole = 15 }
    public enum DeviceStatus { Error = 0, Sucess = 1, DeviceAlreadyHave = 2}
    public enum AvatarStatus { Error = 0, Sucess = 1 }
    public enum UserEventStatus { Error = 0, Sucess = 1 }
    public enum AddEditAvatarStatus { Error = 0, Sucess = 1 }
    public enum BookRatingStatus { Error = 0, Sucess = 1 }
    public enum StudentsImportStatus { Error = 0, Sucess = 1, InvalidSchool = 2 }
    public enum ReadBookStatus { Error = 0, SubscriptionExpires = 1, AlreadyDownloaded = 2, NoMoreCopiesAllowed = 3, Download = 4 }
    public enum UsernameRecoveryStatus { Error = 0, Success = 1, NoUserexist = 2, AccountIsNotActive = 3, AccountIsDisabled = 4 }
    public enum SchoolRegistrationEmailStatus { Error = 0, Success = 1, NoSchoolExist = 2 }
    public enum SchoolAdminDisableStatus { Error = 0, Success = 1, LastAdminDeletionNotallowed = 2, NotASchoolAdmin = 3 }
    public enum BookDisableStatus { Error = 0, Success = 1, NoBookExist = 2 }
    public enum ElibAdminDisableStatus { Error = 0, Success = 1, NotAElibAdmin = 2 }
    public enum SchoolDisableStatus { Error = 0, Success = 1, SchoolNotExist = 2, DeletionNotallowed = 3 }
    public enum ParentStudentDisableStatus { Error = 0, Success = 1, UserNotExist = 2, ParentDeletionNotallowed = 3 }
    public enum Report6Status { Error = 0, Filter = 1, Report = 2 }
    public enum BookStatus { Error = 0, Success = 1, NoBookExists = 2,BookNotRead=3,NoActivity=4 }
}
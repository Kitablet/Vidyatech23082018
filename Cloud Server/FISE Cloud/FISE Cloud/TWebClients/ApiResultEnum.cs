namespace FISE_Cloud.TWebClients
{
    public enum UserStatusEnum { Error = 0, Success = 1, UserAlreadyRegistered = 2, UserAccountNotExist = 3, WrongCredentails = 4, InvalidToken = 5 }
}
public enum SchoolStatusEnum { Error = 0, Success = 1, AccountNotFound = 2, AlreadyHaveSchool = 3, NoSchoolFound = 4, AlreadyHaveSchoolUId=5 }
public enum StudentStatusEnum { Error = 0, Success = 1 }

public enum ClassStatusEnum { Error = 0, Sucess = 1, ClassAlreadyExist = 2, NoClassFound = 3, NoSchoolFound = 4 }
public enum GenericStatusEnum { Error = 0, Sucess = 1, Other = 3 }
public enum APIStatusEnum { Sucess = 1, NoLiccenseAttached = 2, LicenseExpried = 3, InvalidLicenseKey = 4, LicenseAlreadyAssigned = 5, UserNotRegistered = 6, UserAlreadyRegisted = 7, Error = 8, LicenseLanguageMisMatch = 9, UserAccountNotExist = 10, WrongCredentails = 11, SuccessfulLoginWithDemoLicense = 12, SyncPeriodOver = 13, DuplicateProductRegistration = 14, InvalidUserRole = 15 }
public enum SchoolRegistrationEmailStatus { Error = 0, Success = 1, NoSchoolExist = 2 }
public enum SchoolAdminDisableStatus { Error = 0, Success = 1, LastAdminDeletionNotallowed = 2, NotASchoolAdmin = 3 }
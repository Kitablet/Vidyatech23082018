using FISE_API.Services;

namespace FISE_API.Tasks
{
    public class UserDataSyncTask : ITask
    {
        public void Execute()
        {
           UserDataSyncService _userDataSyncService = new UserDataSyncService();
            _userDataSyncService.SyncUserData();
        }  
    }
    public class SyncUserDataBackLogTask : ITask
    {
        public void Execute()
        {
            UserDataSyncService _userDataSyncService = new UserDataSyncService();
            _userDataSyncService.MoveSyncDataToBackLog();
        }
    }
}
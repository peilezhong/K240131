
using Client.Starter;

public class Entry : MonoSingletonDontDestroy<Entry>
{
    private void Awake()
    {
        HotUpdateHandler handler = HotUpdate.Instance.Initialize();
        handler.onUpdateSuccess += onUpdateSuccess;
        handler.onUpdateFailed += onUpdateFailed;
    }

    private void onUpdateSuccess()
    {

    }

    private void onUpdateFailed()
    {

    }
}

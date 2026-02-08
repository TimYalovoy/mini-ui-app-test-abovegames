using System.Collections;

namespace MainApp.Cache
{
    /// <summary>
    /// Base class of Validation.
    /// </summary>
    public abstract class CacheValidationHandler
    {
        protected CacheValidationHandler nextHandler;

        public CacheValidationHandler SetNext(CacheValidationHandler handler)
        {
            nextHandler = handler;
            return handler;
        }

        public abstract IEnumerator Handle(RemoteTextureInfo textureInfo, FileCache cache);

        protected IEnumerator HandleNext(RemoteTextureInfo textureInfo, FileCache cache)
        {
            if (nextHandler != null)
            {
                yield return nextHandler.Handle(textureInfo, cache);
            }
        }
    }
}
namespace SecretSettings.Rewriters;

internal interface IElementContainer<T>
{
    static abstract T Deserialize(string data);
}

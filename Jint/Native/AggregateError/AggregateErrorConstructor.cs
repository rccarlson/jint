using Jint.Native.Error;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Native.TypedArray;
using Jint.Runtime;
using Jint.Runtime.Descriptors;

namespace Jint.Native.AggregateError;

/// <summary>
/// https://tc39.es/ecma262/#sec-aggregate-error-constructor
/// </summary>
internal sealed class AggregateErrorConstructor : FunctionInstance, IConstructor
{
    private static readonly JsString _name = new("AggregateError");

    internal AggregateErrorConstructor(
        Engine engine,
        Realm realm,
        ErrorConstructor errorConstructor)
        : base(engine, realm, _name)
    {
        _prototype = errorConstructor;
        PrototypeObject = new AggregateErrorPrototype(engine, realm, this, errorConstructor.PrototypeObject);
        _length = new PropertyDescriptor(JsNumber.PositiveTwo, PropertyFlag.Configurable);
        _prototypeDescriptor = new PropertyDescriptor(PrototypeObject, PropertyFlag.AllForbidden);
    }

    private AggregateErrorPrototype PrototypeObject { get; }

    protected internal override JsValue Call(JsValue thisObject, JsValue[] arguments)
    {
        return Construct(arguments, this);
    }

    ObjectInstance IConstructor.Construct(JsValue[] arguments, JsValue newTarget) => Construct(arguments, newTarget);

    /// <summary>
    /// https://tc39.es/ecma262/#sec-nativeerror
    /// </summary>
    private ObjectInstance Construct(JsValue[] arguments, JsValue newTarget)
    {
        var errors = arguments.At(0);
        var message = arguments.At(1);
        var options = arguments.At(2);

        var o = OrdinaryCreateFromConstructor(
            newTarget,
            static intrinsics => intrinsics.AggregateError.PrototypeObject,
            static (Engine engine, Realm _, object? _) => new JsError(engine));

        if (!message.IsUndefined())
        {
            var msg = TypeConverter.ToString(message);
            o.CreateNonEnumerableDataPropertyOrThrow("message", msg);
        }

        o.InstallErrorCause(options);

        var errorsList = TypedArrayConstructor.IterableToList(_realm, errors);
        o.DefinePropertyOrThrow("errors", new PropertyDescriptor(_realm.Intrinsics.Array.CreateArrayFromList(errorsList), configurable: true, enumerable: false, writable: true));

        return o;
    }
}

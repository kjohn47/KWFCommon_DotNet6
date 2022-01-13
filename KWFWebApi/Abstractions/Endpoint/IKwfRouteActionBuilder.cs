namespace KWFWebApi.Abstractions.Endpoint
{
    using Microsoft.AspNetCore.Http;

    public delegate Task<IResult> ResultDelegate();

    public delegate Task<IResult> ResultDelegate<in T0>
        (T0 arg0);

    public delegate Task<IResult> ResultDelegate<in T0, in T1>
        (T0 arg0, T1 arg1);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2>
        (T0 arg0, T1 arg1, T2 arg2);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5, in T6>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);

    public delegate Task<IResult> ResultDelegate<in T0, in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15>
        (T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);

    public interface IKwfRouteActionBuilder
    {
        IKwfRouteBuilderResult SetAction
            (Func<IKwfEndpointHandler, ResultDelegate> action);
        IKwfRouteBuilderResult SetAction<T0>
            (Func<IKwfEndpointHandler, ResultDelegate<T0>> action);
        IKwfRouteBuilderResult SetAction<T0, T1>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> action);
        IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
            (Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> action);
    }
}

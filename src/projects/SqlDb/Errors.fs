namespace SqlDb

type ErrorMessage = ErrorMessage of string

module ErrorMessage =
    let asString (ErrorMessage msg) = msg

type Errors =
| GenericError of ErrorMessage
| DbUpEngineError of ErrorMessage*exn
| CommandError of CommandName*exn
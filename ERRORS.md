## Debugging issues

This plugin supports basic logging. When problems arise a log file will be written out to your `Documents/Nasdaq/Excel/logs` folder. In this folder you can find a trace of the errors that have occurred and can be used for helping to debug issues.

_There are rare cases when logs files cannot be generated due to security settings or potentially unhandled exceptions._

## Why do things hang/crash?

There are a number of common reasons in the plugin why things hang/crash. They essentially boil down to one of the following:

* Threading - Doing something silly on or with a thread and/or not cleaning up properly
* Plugin exceptions - Excel and or the COM interface is extremely bad at handling any complex exceptions from our UDFs.
* Async - Understand that excel often is `busy` and you must wait and then retry if your command does not succeed the first time.

### Threading

Excel is generally single threaded but can use multiple threads for calculations. This can get really complicated if you try to send multiple commands at it from different threads. You can also run into nasty issues with the UDFs constantly triggering other UDFs to update if your not careful. This can lead to an infinite loop which hangs the app.

### Async

Since the main excel thread is single threaded it can only do one thing at a time. That means that if you send a command its going to try and process it to completion/error before running another one. Otherwise it will send you an error immediately. This can really get you into trouble if you are:

* Trying to send excel a command from within some code triggered by another command
* You expect Excel to respond immediately with success and don't handle errors

In general you need to figure out which exceptions should be retried and which should not.

## Excel COM Errors List

Below is a list of common Excel COM interaction errors that may occur when the plugin is interacting with excel directly.

| CODE | Integer | Name | Meaning |
| ------------- | ------------- | ------------- | ------------- |
| 0x800AC472 | -2146777998 | VBA_E_IGNORE | The error that is returned whenever an object model call is invoked while the property browser is suspended.  Or to put it another way, when Excel developers want to suspend the object model, they suspend the property browser. |
| 0x8001010A | -2147417846 | RPC_E_SERVERCALL_RETRYLATER | The remote server (Excel) is busy and our plugin cannot interact with it atm. We need to wait and try again. |
| 0x800A01A8 | -2146827864 | | Seems to occur when no cells are selected but you are trying to reference one in a range. |
| 0x800A03EC | -2146827284 | | |
| 0x80020005 | -2147352571 | DISP_E_TYPEMISMATCH | Their is a type mismatch between two objects. |
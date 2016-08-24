# Excel COM Errors List

| CODE | Integer | Name | Meaning |
| ------------- | ------------- | ------------- | ------------- |
| 0x800AC472 | -2146777998 | VBA_E_IGNORE | The error that is returned whenever an object model call is invoked while the property browser is suspended.  Or to put it another way, when Excel developers want to suspend the object model, they suspend the property browser. |
| 0x8001010A | -2147417846 | RPC_E_SERVERCALL_RETRYLATER | The remote server (Excel) is busy and our plugin cannot interact with it atm. We need to wait and try again. |
| 0x800A01A8 | -2146827864 | | Seems to occur when no cells are selected but you are trying to reference one in a range. |
| 0x800A03EC | | | |
| 0x80020005 | -2147352571 | DISP_E_TYPEMISMATCH | Their is a type mismatch between two objects. |
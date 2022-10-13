import { AbstractControl, FormGroup, ValidationErrors, ValidatorFn } from "@angular/forms";

/*
1. only one input argument is expected, which is of type AbstractControl
2. the validator function can obtain the value to be validated via the control.value property
3. the validator function needs to return null if no errors were found in the field value, meaning that the value is valid
4. if any validation errors are found, the function needs to return an object of type ValidationErrors 
5. the ValidationErrors object can have as properties the multiple errors found (usually just one), and as values the details about each error.
6. the value of the ValidationErrors object can be an object with any properties that we want, allowing us to provide a lot of useful information about the error if needed

used for any thing where we need to match two fields like password and confirm password
*/

export class MustMatchValidator{
    constructor() { }

    //apply at the formGroup level: MustMatchValidator.mustMatch('password', 'confirmPassword')
    //to check: password.errors?.mismatch
    static mustMatch(source: string, target: string) : ValidatorFn {
        return (control: AbstractControl) : ValidationErrors | null => {
            
            const sourceControl = control.get(source);
            const targetControl = control.get(target);
            
            if (!targetControl || !sourceControl)
                return null;

            if (targetControl.errors && !targetControl.errors["mustMatch"])
                return null;

            //check 
            if (sourceControl && targetControl && sourceControl.value !== targetControl.value) {
                targetControl.setErrors({ mustMatch: true });
                return ({ mustMatch: true });
            }

            targetControl.setErrors(null);
            return null;
            
        };
    }
}
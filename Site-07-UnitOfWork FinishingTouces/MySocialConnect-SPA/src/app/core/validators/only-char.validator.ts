import { AbstractControl, ValidationErrors, ValidatorFn } from "@angular/forms";

/*
1. only one input argument is expected, which is of type AbstractControl
2. the validator function can obtain the value to be validated via the control.value property
3. the validator function needs to return null if no errors were found in the field value, meaning that the value is valid
4. if any validation errors are found, the function needs to return an object of type ValidationErrors 
5. the ValidationErrors object can have as properties the multiple errors found (usually just one), and as values the details about each error.
6. the value of the ValidationErrors object can be an object with any properties that we want, allowing us to provide a lot of useful information about the error if needed
*/

export class OnlyCharValidator{
    constructor() { }

    //to check: controlName.errors?.onlyChar
    static onlyChar(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const value = control.value;
            //empty good
            if (value === '') return null;
            
            const regEx = new RegExp('^[a-zA-z]*$');
            const isChars = regEx.test(value);

            //match good
            if (isChars) return null;

            //no match bad
            return { onlyChar: true };
        };
    }

    //to check: controlName.errors?.onlyCharWithSpace
    static onlyCharWithSpace(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            const value = control.value;
            //empty good
            if (value === '') return null;

            const regEx = new RegExp('^[a-zA-z ]*$');
            const isChars = regEx.test(value);

            //match good
            if (isChars) return null;

            //no match bad 
            return { onlyCharWithSpace: true };
        };
    }

}
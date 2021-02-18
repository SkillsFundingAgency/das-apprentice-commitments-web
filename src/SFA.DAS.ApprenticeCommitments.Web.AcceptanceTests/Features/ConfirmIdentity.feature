@outerApi
Feature: ConfirmIdentity
	As a apprentice who wants to view his commitment statements
	If he hasn't confirmed his identity

Scenario: The apprentice is authenticated and should see the verify identity page
	Given the apprentice has logged in
	And the apprentice has not verified their identity
	When accessing the "ConfirmYourIdentity" page
	Then the response status code should be Ok
	And the apprentice should see the verify identity page

Scenario: The apprentice is authenticated and should be redirected to the overview page
	Given the apprentice has logged in
	And the apprentice has verified their identity
	When accessing the "ConfirmYourIdentity" page
	And the apprentice should be shown the "Overview" page

Scenario: The apprentice enters valid identity information
	Given the apprentice has logged in
	And the apprentice has not verified their identity
	And the API will accept the identity
	When the apprentice verifies their identity with
	| First name | Last name  | Date of Birth | National Insurance Number |
	| Bob        | bobbertson | 2000-01-30    | AB123456C                 |
	Then verification is successful
	#And the apprentice should see the verify identity page

Scenario: The apprentice enters invalid identity information
	Given the apprentice has logged in
	And the apprentice has not verified their identity
	And the API will reject the identity with the following errors
	| Property Name             | Error Message |
	| FirstName                 | not valid     |
	| LastName                  | not valid     |
	| DateOfBirth               | not valid     |
	| NationalInsuranceNumber   | not valid     |
	| SomethingWeDoNotKnowAbout | is very wrong |
	When the apprentice verifies their identity with
	| First name | Last name  | Date of Birth | National Insurance Number |
	| Bob        | bobbertson | 2000-01-01    | lars                      |
	Then verification is not successful
	And the apprentice should see the following error messages
	| Property Name           | Error Message                        |
	| FirstName               | Enter your first name                |
	| LastName                | Enter your last name                 |
	| DateOfBirth             | Enter your date of birth             |
	| NationalInsuranceNumber | Enter your National Insurance Number |


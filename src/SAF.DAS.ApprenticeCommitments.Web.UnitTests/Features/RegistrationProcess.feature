@outerApi
Feature: RegistrationProcess
	As an apprentice who wants to view my commitment statements
	If I haven't confirmed my identity

Scenario: The apprentice has just created a login, but not an apprentice account
	Given the apprentice has logged in but not created their account
	When accessing the "Register/bob" page 
	Then the apprentice should be redirected to the personal details page
	And the registration code should be "bob"

Scenario: The apprentice loads the registration page
	Given the apprentice has logged in but not created their account
	When accessing the "Register/bob" page
	Then the apprentice should be redirected to the personal details page
	And the apprentice marks the "bob" registration as seen

#Scenario: The registration is not seen if there is no code
#	Given the apprentice has logged in but not created their account
#	When accessing the "Register" page
#	Then the apprentice should see the personal details page
#	And the apprentice does not try to mark the registration as seen
#
#Scenario: The apprentice enters their personal details for the first time
#	Given the apprentice has logged in but not created their account
#	When accessing the "Account" page
#	Then the response status code should be Ok
#	And the apprentice should see the personal details page
#
#Scenario: The apprentice wants to update their account details
#	Given the apprentice has logged in
#	And the apprentice has created their account
#	When accessing the "Account" page
#	Then the response status code should be Ok
#	And the apprentice does not try to mark the registration as seen
#	And the apprentice should see the personal details page
#	And the apprentice sees their previously entered details
#
#Scenario: The apprentice updates their account details
#	Given the apprentice has logged in
#	And the apprentice has created their account
#	And the API will accept the account update
#	When the apprentice updates their account with
#	| First name | Last name  | EmailAddress    | Date of Birth |
#	| Bob        | bobbertson | bob@example.com | 2000-01-30    |
#	Then the apprentice should be shown the Home page
#
#Scenario: The apprentice creates their account accepts the terms of use
#	Given the apprentice has logged in but not created their account
#	And the apprentice has a registration code
#	And the API will accept the account
#	When the apprentice creates their account with
#	| First name | Last name  | EmailAddress    | Date of Birth |
#	| Bob        | bobbertson | bob@example.com | 2000-01-30    |
#	Then the apprentice account is created
#	And the apprentice is shown the Terms of Use
#	And the authentication includes the apprentice's names: "Bob" and "bobbertson"
#
#Scenario: The apprentice creates their account and matches to the apprenticeship
#	Given the apprentice has logged in but not matched their account
#	And the apprentice has a registration code
#	And the API will match the apprenticeship
#	When the apprentice accepts the terms of use
#	Then the apprentice is matched to the apprenticeship
#	And the apprentice should be shown the Home page with a Matched notification
#	And the authentication includes the terms of use
#
#Scenario: The apprentice creates their account and does not match to the apprenticeship
#	Given the apprentice has logged in but not matched their account
#	And the apprentice has a registration code
#	And the API will not match the apprenticeship
#	When the apprentice accepts the terms of use
#	Then the apprentice should be shown the Home page with a Not Matched notification
#
#Scenario: The apprentice registers a new apprenticeship to their existing account
#	Given the apprentice has logged in
#	And the apprentice has created their account
#	And the API will accept the account
#	And the API will match the apprenticeship
#	When the apprentice accepts the terms of use
#	Then the apprentice should be shown the Home page with a Matched notification
#
#Scenario: The apprentice enters invalid identity information
#	Given the apprentice has logged in but not created their account
#	And the API will reject the identity with the following errors
#	| Property Name             | Error Message |
#	| FirstName                 | not valid     |
#	| LastName                  | not valid     |
#	| DateOfBirth               | not valid     |
#	| SomethingWeDoNotKnowAbout | is very wrong |
#	When the apprentice creates their account with
#	| First name | Last name | Date of Birth |
#	|            |           | 1000-01-01    |
#	Then verification is not successful
#	And the apprentice should see the following error messages
#	| Property Name | Error Message            |
#	| FirstName     | Enter your first name    |
#	| LastName      | Enter your last name     |
#	| DateOfBirth   | Enter your date of birth |
#	|               | Something went wrong     |
#	And the DateOfBirth field is editable
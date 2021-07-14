@outerApi
Feature: ConfirmIdentity
	As an apprentice who wants to view my commitment statements
	If I haven't confirmed my identity

Scenario: The apprentice is authenticated and should see the verify identity page for the first time
	Given the apprentice has logged in
	And the apprentice has not verified their identity
	When accessing the "ConfirmYourIdentity" page
	Then the response status code should be Ok
	And the apprentice should see the verify identity page
	And the apprentice marks the registration as seen

	Scenario: The apprentice is authenticated and should see the verify identity page (again)
	Given the apprentice has logged in
	And the apprentice has not verified their identity, but has seen this page
	When accessing the "ConfirmYourIdentity" page
	Then the response status code should be Ok
	And the apprentice should see the verify identity page
	And the apprentice does not try to mark the registration as seen

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
	| First name | Last name  | Date of Birth |
	| Bob        | bobbertson | 2000-01-30    |
	Then verification is successful

Scenario: The apprentice enters invalid identity information
	Given the apprentice has logged in
	And the apprentice has not verified their identity
	And the API will reject the identity with the following errors
	| Property Name             | Error Message                        |
	| FirstName                 | not valid                            |
	| LastName                  | not valid                            |
	| DateOfBirth               | not valid                            |
	| SomethingWeDoNotKnowAbout | is very wrong                        |
	|                           | Sorry, can't validate you, try again |
	When the apprentice verifies their identity with
	| First name | Last name  | Date of Birth |
	| Bob        | bobbertson | 2000-01-01    |
	Then verification is not successful
	And the apprentice should see the following error messages
	| Property Name | Error Message                        |
	| FirstName     | Enter your first name                |
	| LastName      | Enter your last name                 |
	| DateOfBirth   | Enter your date of birth             |
	|               | Something went wrong                 |
	|               | Sorry, can't validate you, try again |

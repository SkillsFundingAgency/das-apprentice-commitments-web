@outerApi
Feature: CreateApprenticeAccount
	As an apprentice who wants to view my commitment statements
	If I haven't confirmed my identity

Scenario: The apprentice is should see their account page to create their account
	Given the apprentice has logged in
	And the apprentice has not created their account
	When accessing the "ConfirmYourPersonalDetails?handler=signup&registrationCode=bob" page
	Then the response status code should be Ok
	And the apprentice should see the personal details page
	And the apprentice marks the registration as seen

Scenario: The apprentice is should see their account page to update their account
	Given the apprentice has logged in
	And the apprentice has created their account
	When accessing the "ConfirmYourPersonalDetails?handler=signup" page
	Then the response status code should be Ok
	Then the apprentice should see the personal details page

Scenario: The apprentice creates their account and matches to the apprenticeship
	Given the apprentice has logged in
	And the apprentice has not created their account
	And the API will accept the account
	And the API will match the apprenticeship
	When the apprentice creates their account with
	| First name | Last name  | EmailAddress    | Date of Birth |
	| Bob        | bobbertson | bob@example.com | 2000-01-30    |
	Then the apprentice account is updated
	And the apprentice should be shown the Home page

Scenario: The apprentice creates their account and does not match to the apprenticeship
	Given the apprentice has logged in
	And the apprentice has not created their account
	And the API will accept the account
	And the API will not match the apprenticeship
	When the apprentice creates their account with
	| First name | Last name  | EmailAddress    | Date of Birth |
	| Bob        | bobbertson | bob@example.com | 2000-01-30    |
	Then the apprentice account is updated
	And the apprentice should be shown the Home page with a Not Matched notification

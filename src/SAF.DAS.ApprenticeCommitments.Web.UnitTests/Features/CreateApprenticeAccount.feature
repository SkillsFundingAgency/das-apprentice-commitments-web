@outerApi
Feature: CreateApprenticeAccount
	As an apprentice who wants to view my commitment statements
	If I haven't confirmed my identity

Scenario: The apprentice has just created a login account
	Given an unverified logged in user
	And the apprentice has not created their account
	When accessing the "Register?registrationCode=bob" page 
	Then the apprentice should see the personal details page
	And the registration code should be "bob"

Scenario: The enters their personal details for the first time
	Given the apprentice has logged in
	And the apprentice has not created their account
	When accessing the "ConfirmYourPersonalDetails?handler=register" page
	Then the response status code should be Ok
	And the apprentice marks the registration as seen
	And the apprentice should see the personal details page

Scenario: The apprentice updates their account details
	Given the apprentice has logged in
	And the apprentice has created their account
	When accessing the "ConfirmYourPersonalDetails?handler=register" page
	Then the response status code should be Ok
	And the apprentice does not try to mark the registration as seen
	And the apprentice should see the personal details page

Scenario: The apprentice creates their account and matches to the apprenticeship
	Given the apprentice has logged in
	And the apprentice has not created their account
	And the API will accept the account
	And the API will match the apprenticeship
	When the apprentice creates their account with
	| First name | Last name  | EmailAddress    | Date of Birth |
	| Bob        | bobbertson | bob@example.com | 2000-01-30    |
	Then the apprentice account is updated
	And the apprentice is matched to the apprenticeship
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

Scenario: The apprentice registers a new apprenticeship to their existing account
	Given the apprentice has logged in
	And the apprentice has created their account
	And the API will accept the account
	And the API will match the apprenticeship
	When the apprentice creates their account with
	| First name | Last name  | EmailAddress    | Date of Birth |
	| Bob        | bobbertson | bob@example.com | 2000-01-30    |
	Then the apprentice account is updated
	And the apprentice should be shown the Home page

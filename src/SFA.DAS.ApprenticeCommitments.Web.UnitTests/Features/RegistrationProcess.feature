@outerApi
Feature: RegistrationProcess
	As an apprentice who wants to view my commitment statements
	If I haven't confirmed my identity

Scenario: The apprentice has just created a login, but not an apprentice account
	Given the apprentice has logged in but not created their account
	When accessing the "Register/bob" page 
	Then the apprentice should be redirected to the personal details page
	And the registration code should be "bob"

Scenario: The registration is not seen if there is no code
	Given the apprentice has logged in but not created their account
	When accessing the "Register" page
	Then the apprentice should be redirected to the personal details page
	And the apprentice does not try to mark the registration as seen

Scenario: The apprentice creates their account and matches to the apprenticeship
	Given the apprentice has logged in but not matched their account
	And the apprentice has a registration code
	And the API will match the apprenticeship
	When accessing the "Register" page 
	Then the apprentice is matched to the apprenticeship
	And the apprentice should be shown the Home page with a Matched notification

Scenario: The apprentice creates their account and does not match to the apprenticeship
	Given the apprentice has logged in but not matched their account
	And the apprentice has a registration code
	And the API will not match the apprenticeship
	When accessing the "Register" page 
	Then the apprentice should be shown the Home page with a Not Matched notification

Scenario: The apprentice registers a new apprenticeship to their existing account
	Given the apprentice has logged in
	And the apprentice has created their account
	And the apprentice has a registration code
	And the API will match the apprenticeship
	When accessing the "Register" page 
	Then the apprentice should be shown the Home page with a Matched notification
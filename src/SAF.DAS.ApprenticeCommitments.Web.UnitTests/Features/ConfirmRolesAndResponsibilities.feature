@outerApi
Feature: ConfirmRolesAndResponsibilities
	As an apprentice i want to view my Roles and Responsibilities
	So I can confirm them

Scenario: The apprentice is authenticated and should see the Roles and Responsibilities
	Given the apprentice has logged in
	And the apprentice has not verified their Roles and Responsibilities
	When accessing the RolesAndResponsibilities page
	Then the response status code should be Ok
	And the apprentice should see the Roles and Responsibilities
	And the link is pointing to the confirm page

Scenario: The apprentice is authenticated and confirms the Roles and Responsibilities
	Given the apprentice has logged in
	And the apprentice has not verified their Roles and Responsibilities
	And the apprentice confirms their Roles and Responsibilities
	When submitting the RolesAndResponsibilities page
	Then the apprenticeship is updated to show the a 'true' confirmation
	And the user should be redirected back to the overview page

Scenario: The apprentice is authenticated and refuses to confirm the Roles and Responsibilities
	Given the apprentice has logged in
	And the apprentice has not verified their Roles and Responsibilities
	And the apprentice refuses to confirm their Roles and Responsibilities
	When submitting the RolesAndResponsibilities page
	Then the apprenticeship is updated to show the a 'false' confirmation
	And the user should be redirected to the cannot confirm apprenticeship page

Scenario: The apprentice is authenticated and presses the Confirm actions without select yes or no
	Given the apprentice has logged in
	And the apprentice has not verified their Roles and Responsibilities
	And the apprentice doesn't select an option
	When submitting the RolesAndResponsibilities page
	Then the response status code should be Ok
	And the model should contain an error message
	And the apprentice should see the Roles and Responsibilities
	And the link is pointing to the confirm page

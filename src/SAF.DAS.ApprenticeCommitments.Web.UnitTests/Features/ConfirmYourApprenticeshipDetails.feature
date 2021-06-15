@outerApi
Feature: ConfirmYourApprenticeshipDetails
	As an apprentice who wants to view my apprenticeship details
	So I can confirm them

Scenario: The apprentice is authenticated and should see the Apprenticeship details
	Given the apprentice has logged in
	And the apprentice has not verified their apprenticeship details
	When accessing the YourApprenticeshipDetails page
	Then the response status code should be Ok
	And the apprentice should see the course name
	And the apprentice should see the course level
	And the apprentice should see the course option
	And the apprentice should see the duration in months
	And the apprentice should see the planned start date
	And the apprentice should see the planned end date
	And the link is pointing to the confirm page

Scenario: The apprentice is authenticated and confirms their apprenticeship details
	Given the apprentice has logged in
	And the apprentice has not verified their apprenticeship details
	And the apprentice confirms their apprenticeship details
	When submitting the YourApprenticeshipDetails page
	Then the apprenticeship is updated to show a 'true' confirmation
	And the user should be redirected back to the overview page

Scenario: The apprentice is authenticated and states that these are not their apprenticeship details
	Given the apprentice has logged in
	And the apprentice has not verified their apprenticeship details
	And the apprentice states these are not their apprenticeship details
	When submitting the YourApprenticeshipDetails page
	Then the apprenticeship is updated to show a 'false' confirmation
	And the user should be redirected to the cannot confirm apprenticeship page

Scenario: The apprentice is authenticated and presses the Confirm actions without select yes or no
	Given the apprentice has logged in
	And the apprentice has not verified their apprenticeship details
	And the apprentice doesn't select an option
	When submitting the YourApprenticeshipDetails page
	Then the response status code should be Ok
	And the model should contain an error message
	And the apprentice should see the course name
	And the apprentice should see the course level
	And the apprentice should see the course option
	And the apprentice should see the duration in months
	And the apprentice should see the planned start date
	And the apprentice should see the planned end date
	And the link is pointing to the confirm page

Scenario: The apprentice is authenticated and has negatively confirmed
	Given the apprentice has logged in
	And the apprentice has negatively confirmed their apprenticeship details
	When accessing the YourApprenticeshipDetails page
	Then the response status code should be Ok
	And the apprentice should see the course name
	And the user should see the confirmation options

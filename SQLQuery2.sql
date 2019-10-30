SELECT c.Name, s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId,
                                                i.Id, i.FirstName, i.LastName, i.SlackHandle, i.CohortId, i.Specialty
                                        FROM Cohort c LEFT JOIN Student s ON s.CohortId = c.Id
                                                      --RIGHT JOIN Instructor i ON i.CohortId = c.Id

SELECT c.Name, s.Id, s.FirstName, s.LastName, s.SlackHandle, s.CohortId,
		i.Id, i.FirstName, i.LastName, i.SlackHandle, i.CohortId, i.Specialty
FROM Cohort c INNER JOIN Student s ON s.CohortId = c.Id
			INNER JOIN Instructor i ON i.CohortId = c.Id;

SELECT	c.Id as 'CohortId', c.Name,
		s.Id as 'StudentId', s.FirstName, s.LastName, s.SlackHandle, s.CohortId
FROM Cohort c JOIN Student s ON s.CohortId = c.Id
import { useMemo, type JSX } from "react"
import { Table } from "react-bootstrap"
import type { UserInfoModel } from "@core/models/user-models"
import './ReportTable.css'
import { CheckSquare, X } from "lucide-react"

type StudentAttendance = { 
  studentFIO: string, 
  checks: boolean[], 
  studentId: number,
}

export interface ILessonStudentInfo {
  lessonId: number,
  time: string,
  students: {
    student: UserInfoModel
    checked: boolean
  }[]
}

export interface ReportTableProps {
  lessons: ILessonStudentInfo[]
}

export function ReportTable({ lessons }: ReportTableProps): JSX.Element {
  const students = useMemo(() => {
    const result: StudentAttendance[] = []
    for (const lesson of lessons) {
      lesson.students.forEach(({ student, checked }) => {
        const fio = `${student.lastName} ${student.firstName}`
        const existing = result.find(s => s.studentFIO === fio)
        if (!existing) {
          result.push({ studentFIO: fio, checks: [checked], studentId: student.externalId })
        } else {
          existing.checks.push(checked)
        }
      })
    }
    return result
  }, [lessons])

  return (
    <div className="report-table shadow-sm rounded bg-white p-3">
      <div className="table-responsive">
        <Table bordered hover className="align-middle text-center table-striped">
          <thead className="table-primary">
            <tr>
              <th style={{ width: '200px', minWidth: '200px', background:'#6a11cb22' }}>ФИО</th>
              {lessons.map((lesson, index) => (
                <th style={{width: '20px', background:'#6a11cb22'}} key={`head-${index}`}>
									{convertToDDMM(lesson.time)}
								</th>
              ))}
              <th style={{width: '60px', background:'#6a11cb22'}}>%</th>
              <th style={{background:'#6a11cb22'}} className="border-0" />
            </tr>
          </thead>
          <tbody>
            {students
              .sort((a, b) => a.studentFIO.localeCompare(b.studentFIO))
              .map((info, index) => (
                <tr key={`row-${index}`}>
                  <td className="text-start">{info.studentFIO}</td>
                  {info.checks.map((checked, i) => (
                    <td key={`cell-${index}-${i}`}>
                      {checked ? (
                        <CheckSquare className="text-success" size={20} />
                      ) : (
                        <X className="text-danger" size={20} />
                      )}
                    </td>
                  ))}
                  <td>
                    {((info.checks.filter(Boolean).length / lessons.length) * 100).toFixed(2)}%
                  </td>
                </tr>
              ))}
          </tbody>
          <tfoot className="table-light">
            <tr>
              <td className="fw-bold text-decoration-underline">Статистика:</td>
              {lessons.map((lesson, index) => {
                const stat = lesson.students.filter(s => s.checked).length / lesson.students.length
                return (
                  <td key={`stat-${index}`}>
                    {(stat * 100 || 0).toFixed(2)}%
                  </td>
                )
              })}
            </tr>
          </tfoot>
        </Table>
      </div>
    </div>
  )
}

function convertToDDMM(dateTime: string): string {
  const date = new Date(dateTime)
  const day = String(date.getDate()).padStart(2, '0')
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const hour = String(date.getHours()).padStart(2, '0')
  const minute = String(date.getMinutes()).padStart(2, '0')
  return `${day}.${month} ${hour}:${minute}`
}

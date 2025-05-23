import Loading from "@components/loading/Loading";
import type { LessonInfoModel } from "@core/models/lesson-models";
import { useCourseStore, useLessonStore } from "@core/store/store";
import { useEffect, useState, type JSX } from "react";
import { Alert, Table } from "react-bootstrap";

interface Props {
  lesson: LessonInfoModel
}

const getBadgeColor = (groupMode: string): string => {
  switch (groupMode) {
    case 'None': return 'secondary';
    case 'Isolated': return 'primary';
    case 'Visible': return 'success';
    default: return 'dark';
  }
};

export default function Attendances({ lesson }: Props): JSX.Element {
  
  const { course } = useLessonStore()
  const { students, fetchStudents, error, isLoading } = useCourseStore()

  useEffect(() => {
    if (!course) return;
    (async () => {
      await fetchStudents(course.externalId)
    })().catch(error => console.log(error))

  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [course])

  return (
    <div>
      <Loading isLoading={isLoading}>
        {error && <Alert className="mb-2" variant="danger">{error}</Alert>}
        <Table responsive striped className="align-middle mb-0 w-100" style={{
          backgroundColor: 'transparent',
          overflow: 'auto'
        }}>
          <thead style={{fontSize: '15px', fontWeight: 'normal'}}>
            <tr>
              <th style={{}}>#</th>
              <th style={{width: '180px'}}>ФИО</th>
              <th style={{}}>Группа</th>
              <th style={{}}>Статус</th>
              <th style={{}}></th>
              <th style={{}}></th>
            </tr>
          </thead>
          <tbody style={{fontSize: '14px'}}>
            {students.map((item, index) => (
              <tr key={item.externalId}>
                <td>{index + 1}</td>
                <td>{(() => {
                  const filter = item.description.replace(/<\/?[^>]+(>|$)/g, '')
                  return filter.length > 0 ? <>{filter}</> : 'Обычное занятие'
                })()}</td>
                <td>
                  {
                    item.groupInfo?.groupName ?? 'Для всех'
                  }
                </td>
                <td>
                  { (() => {
                    const attendance = course.attendanceModules.find(att => att.externalId == item.attendanceId)
                    if (!attendance) return <></>
                    return (<>
                      {attendance.name}{' '}
                      <Badge pill bg={getBadgeColor(attendance.groupMode)} className="ms-1">
                        {attendance.groupMode}
                      </Badge>
                    </>)
                  })() }
                </td>
                <td>
                </td>
              </tr>
            ))}
          </tbody>
        </Table>
      </Loading>
    </div>
  )
}
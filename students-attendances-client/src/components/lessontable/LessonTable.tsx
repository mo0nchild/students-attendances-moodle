import type { CourseInfoModel } from "@core/models/course-models";
import type { LessonInfoModel } from "@core/models/lesson-models";
import { BookText, Pencil } from "lucide-react";
import { type JSX } from "react";
import { Badge, Button, Table } from "react-bootstrap";

interface Props {
	lessons: LessonInfoModel[]
	course: CourseInfoModel
  onEdit?: (lesson: LessonInfoModel) => void
  onSelect?: (lesson: LessonInfoModel) => void
}

const getBadgeColor = (groupMode: string): string => {
  switch (groupMode) {
    case 'None': return 'secondary';
    case 'Isolated': return 'primary';
    case 'Visible': return 'success';
    default: return 'dark';
  }
};

export default function LessonTable({ lessons, course, onSelect, onEdit }: Props): JSX.Element {
  return (
		<div className="rounded shadow-sm p-3 bg-white bg-opacity-75" style={{
      overflow: 'auto'
    }}>
      <Table responsive striped className="align-middle mb-0 w-100" style={{
        backgroundColor: 'transparent',
        overflow: 'auto'
      }}>
        <thead style={{fontSize: '15px', fontWeight: 'normal'}}>
          <tr>
            <th style={{}}>#</th>
            <th style={{width: '180px'}}>Название</th>
            <th style={{}}>Начало занятия</th>
            <th style={{}}>Продолж.</th>
            <th style={{width: '100px'}}>Группа</th>
            <th style={{}}>Модуль</th>
            <th style={{}}></th>
            <th style={{}}></th>
          </tr>
        </thead>
        <tbody style={{fontSize: '14px'}}>
          {lessons.map((item, index) => (
            <tr key={item.externalId}>
              <td>{index + 1}</td>
              <td>{(() => {
                const filter = item.description.replace(/<\/?[^>]+(>|$)/g, '')
                return filter.length > 0 ? <>{filter}</> : 'Обычное занятие'
              })()}</td>
              <td>{formatDateTime(item.startTime)}</td>
              <td>{calculateDuration(item.startTime, item.endTime)}</td>
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
                <Button style={{backgroundColor: '#FFFFFF', borderColor: 'grey'}} 
                  onClick={() => {onSelect?.(item)}}  
                >
                  <BookText size={15} color="#000"/>
                </Button>
              </td>
              <td>
                <Button style={{backgroundColor: '#FFFFFF', borderColor: 'goldenrod'}} 
                  onClick={() => {onEdit?.(item)}}  
                >
                  <Pencil size={15} color="goldenrod"/>
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
	)
}
 const formatter = new Intl.DateTimeFormat('ru-RU', {
  day: '2-digit',
  month: '2-digit',
  year: 'numeric',
  hour: '2-digit',
  minute: '2-digit',
  timeZone: 'UTC'
})
function formatDateTime(isoDate: string): string {
	const date = new Date(isoDate);
 
	return `${formatter.format(date)}`;
}

function calculateDuration(start: string, end: string): string {
  const diffMs = new Date(end).getTime() - new Date(start).getTime()

  const diffMinutes = Math.floor(diffMs / (1000 * 60))
  const hours = Math.floor(diffMinutes / 60)
  const minutes = diffMinutes % 60

  return `${hours} ч ${minutes} мин`
}
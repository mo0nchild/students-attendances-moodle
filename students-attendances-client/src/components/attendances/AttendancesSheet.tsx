import Loading from "@components/loading/Loading";
import { ReportTable, type ILessonStudentInfo } from "@components/reporttable/ReportTable";
import type { LessonInfoModel } from "@core/models/lesson-models";
import { useCourseStore, useLessonStore } from "@core/store/store";
import { useEffect, useMemo, useState, type JSX } from "react";
import { useNavigate } from "react-router-dom";

interface Props {
	groupId: number | null
	onClose?: () => void
}

export default function AttendancesSheet({ groupId, onClose }: Props): JSX.Element {
	const fetchLessons = useLessonStore(state => state.fetchLessons)
	const course = useLessonStore(state => state.course)
	const lessons = useLessonStore(state => state.lessons)

	const fetchStudents = useCourseStore(state => state.fetchStudents)
	const students = useCourseStore(state => state.students)

	const [ isLoading, setLoading ] = useState<boolean>(false)
	const navigate = useNavigate()

	const lessonsAttendance = useMemo<ILessonStudentInfo[]>(() => {
		const mapping = (lessonsList: LessonInfoModel[]) => (
			lessonsList.map(item => ({
				lessonId: item.externalId,
				time: item.startTime,
				students: students.map(st => ({
					student: st,
					checked: (() => {
						const att = item.attendances!.find(att => att.studentId == st.externalId)
						return (!att || att.acronym == 'Н') ? false : true
					})()
				}))
			}) as ILessonStudentInfo)
		)
		if (groupId) {
			return mapping(lessons.filter(item => item.groupInfo?.externalId == groupId))
		}
		return mapping(lessons.filter(item => !item.groupInfo))
	}, [lessons, students, groupId])

	useEffect(() => {
		if (!course) {
			navigate('/dashboard')
			return
		}
		setLoading(true);
		(async () => {
			await fetchLessons(course.externalId)
			await fetchStudents(course.externalId)
			setLoading(false)
		})().catch(error => {
			setLoading(false)
			alert(error)
		})
	// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [course])
  	return (
		<div className="p-2 py-4 p-md-4 shadow rounded bg-white bg-opacity-75">
			<div className='d-flex flex-row mb-4 justify-content-between align-items-center gap-4'>
				<h6 className="m-0">Таблица посещений</h6>
				<h6 className="m-0" onClick={() => onClose?.()}
					style={{
						textDecoration: 'underline',
						cursor: 'pointer'
					}}
				>Закрыть</h6>
			</div>
			<Loading isLoading={isLoading}>
				<ReportTable lessons={lessonsAttendance}/>
			</Loading>
		</div>
		
	)
}
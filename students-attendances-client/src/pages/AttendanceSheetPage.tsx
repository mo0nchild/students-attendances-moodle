import Loading from "@components/loading/Loading";
import PageCard from "@components/pagecard/PageCard";
import { ReportTable, type ILessonStudentInfo } from "@components/reporttable/ReportTable";
import type { LessonInfoModel } from "@core/models/lesson-models";
import { useCourseStore, useLessonStore } from "@core/store/store";
import { LucideCircleArrowLeft } from "lucide-react";
import { useEffect, useMemo, useState, type JSX } from "react";
import { useNavigate, useParams } from "react-router-dom";

export default function AttendanceSheetPage(): JSX.Element {
	const fetchLessons = useLessonStore(state => state.fetchLessons)
	const course = useLessonStore(state => state.course)
	const lessons = useLessonStore(state => state.lessons)

	const fetchStudents = useCourseStore(state => state.fetchStudents)
	const students = useCourseStore(state => state.students)

	const [ isLoading, setLoading ] = useState<boolean>(false)
	const { groupId } = useParams()
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
			return mapping(lessons.filter(item => item.groupInfo?.externalId == parseInt(groupId)))
		}
		return mapping(lessons.filter(item => !item.groupInfo))
	// eslint-disable-next-line react-hooks/exhaustive-deps
	}, [lessons, students])

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
		<PageCard>
			<div className="p-2 py-4 p-md-4 shadow rounded bg-white bg-opacity-75">
				<div className='d-flex flex-row mb-4 justify-content-between align-items-center gap-4'>
					<div className='gradient-input-wrapper p-1' onClick={() => navigate('/lessons')} style={{
						cursor: 'pointer'
					}}>
						<LucideCircleArrowLeft color='white' size={26}/>
					</div>
					<h3 className="m-0">Таблица посещений</h3>
				</div>
				<Loading isLoading={isLoading}>
					<ReportTable lessons={lessonsAttendance}/>
				</Loading>
			</div>
		</PageCard>
	)
}